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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
#endregion

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Native
#else
namespace Syncfusion.CompoundFile.XlsIO.Native
#endif
{
  /// <summary>
  /// Storage API wrapper classes provide access to storage data from .NET code.
  /// </summary>
#if !(WINRT || WP )
  [Syncfusion.Documentation.DocumentationExclude()]
#endif
  public class StgStream : System.IO.Stream, IDisposable
  {
    #region Class constants
    /// <summary>
    /// Open storage in read-only mode.
    /// </summary>
    public const STGM DEF_STORE_READONLY = STGM.STGM_READ | STGM.STGM_SHARE_DENY_WRITE;
    /// <summary>
    /// Open storage stream in read-only mode.
    /// </summary>
    public const STGM DEF_STREAM_READONLY = STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE;
    /// <summary>
    /// Create a new stream in storage.
    /// </summary>
    public const STGM DEF_STREAM_CREATE = STGM.STGM_CREATE | STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE;
    /// <summary>
    /// Open storage or stream in ReadWrite mode.
    /// </summary>
    public const STGM DEF_READWRITE = STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE;
    /// <summary>
    /// Default buffer size for stream copying.
    /// </summary>
    private const int DEF_BUFFER_SIZE = 32 * 1024;
    /// <summary>
    /// Options to open storage in read-only mode. Used to open already opened file.
    /// </summary>
    public const STGM DEF_STORAGE_READONLY = STGM.STGM_DIRECT_SWMR
      | STGM.STGM_SHARE_DENY_NONE | STGM.STGM_READ;
    #endregion

    #region Members
    /// <summary>
    /// Reference in COM interface which provides access to stream in storage.
    /// </summary>
    private IStream m_stream;
    /// <summary>
    /// Reference in COM interface which provide access to storage.
    /// </summary>
    private IStorage m_storage;
    /// <summary>
    /// True if class was disposed; otherwise False.
    /// </summary>
    private bool m_bIsDisposed;
    /// <summary>
    /// True if stream supports read operation; otherwise False.
    /// </summary>
    private bool m_bCanRead;
    /// <summary>
    /// True stream supports write operation; otherwise False.
    /// </summary>
    private bool m_bCanWrite;
    /// <summary>
    /// True if stream supports seek operation; otherwise False.
    /// </summary>
    private bool m_bCanSeek;
    /// <summary>
    /// True if stream opened in Transaction mode and on Flush method call
    /// class must commit transaction; otherwise False.
    /// </summary>
    private bool m_bIsTransacted;
    /// <summary>
    /// Length of stream data.
    /// </summary>
    private long m_lLength = -1;
    /// <summary>
    /// List of streams names provided by storage.
    /// </summary>
    private List<string> m_arrStreams = new List<string>();
    /// <summary>
    /// List of storage names found in current storage.
    /// </summary>
    private List<string> m_arrStorages = new List<string>();
    /// <summary>
    /// File name of storage.
    /// </summary>
    private string m_strFileName;
    /// <summary>
    /// Stream name.
    /// </summary>
    private string m_strStreamName;
    /// <summary>
    /// Sub-storage name opened by class.
    /// </summary>
    private string m_strStorageName;
    /// <summary>
    /// Storage Mode: Open or Create.
    /// </summary>
    private STGM m_modeStorage;
    /// <summary>
    /// Stream Mode: Open or Create.
    /// </summary>
    private STGM m_modeStream;
    /// <summary>
    /// Current stream position, used for optimization. Allows users
    /// to skip Seek operations if required.
    /// </summary>
    private long m_lPosition = 0;
    /// <summary>
    /// Represents the locking bytes.
    /// </summary>
    private ILockBytes m_lockBytes;
    #endregion

    #region Properties
    /// <summary>
    /// Indicates if stream supports Read operation. Read-only.
    /// </summary>
    public override bool CanRead
    {
      get
      {
        return m_bCanRead;
      }
    }

    /// <summary>
    /// Indicates if stream supports Seek operation. Read-only.
    /// </summary>
    public override bool CanSeek
    {
      get
      {
        return m_bCanSeek;
      }
    }

    /// <summary>
    /// Indicates if stream supports Write operation. Read-only.
    /// </summary>
    public override bool CanWrite
    {
      get
      {
        return m_bCanWrite;
      }
    }

    /// <summary>
    /// Indicates if stream is opened in Transaction mode. Read-only.
    /// </summary>
    public bool IsTransacted
    {
      get
      {
        return m_bIsTransacted;
      }
    }

    /// <summary>
    /// Length of stream. Read-only.
    /// </summary>
    public override long Length
    {
      get
      {
        return m_lLength;
      }
    }

    /// <summary>
    /// Gets / sets current position of stream.
    /// </summary>
    public override long Position
    {
      get
      {
        return m_lPosition;
      }
      set
      {
        Seek( value, SeekOrigin.Begin );
      }
    }

    /// <summary>
    /// Gets list of stream names found in storage. Read-only.
    /// </summary>
    public string[] Streams
    {
      get
      {
        return m_arrStreams.ToArray();
      }
    }

    /// <summary>
    /// Gets the array of string thet is a storages.
    /// </summary>
    public string[] Storages
    {
      get
      {
        return m_arrStorages.ToArray();
      }
    }
    /// <summary>
    /// Reference in COM interface which provide access to storage.
    /// </summary>
    [CLSCompliant( false )]
    public IStorage COMStorage
    {
      get
      {
        return m_storage;
      }
    }

    /// <summary>
    /// Reference in COM interface which provide access to stream in storage.
    /// </summary>
    [CLSCompliant( false )]
    public IStream COMStream
    {
      get
      {
        return m_stream;
      }
    }
    /// <summary>
    /// Get name of stream opened by the class.
    /// </summary>
    public string StreamName
    {
      get
      {
        return m_strStreamName;
      }
    }
    /// <summary>
    /// Get name of sub storage opened by the class.
    /// </summary>
    public string StorageName
    {
      get
      {
        return m_strStorageName;
      }
    }
    /// <summary>
    /// Gets the IlockBytes interface that represen the locked bytes.
    /// </summary>
    [CLSCompliant( false )]
    public ILockBytes LockBytes
    {
      get
      {
        return m_lockBytes;
      }
    }
    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string FileName
    {
      get
      {
        return m_strFileName;
      }
    }
    #endregion

    #region Finalize methods
#if !(WINRT || WP )
    /// <summary>
    /// Closes the stream.
    /// </summary>
    public override void Close()
    {
      base.Close();

#if DEBUG_STGSTREAM && DOCIO
      Debug.WriteLineIf( m_strStreamName != null, "Stream Closed: " + m_strStreamName, "Closed" );
#endif

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled && m_strStreamName != null,
      //  "Stream Closed: " + m_strStreamName, "Closed" );

      if( m_stream != null )
      {
        m_stream.Commit( ( uint )STGC.STGC_DEFAULT );
        Marshal.FinalReleaseComObject( m_stream );
        GC.SuppressFinalize( m_stream );
      }

      m_stream = null;
      m_strStreamName = null;
    }
#endif
    /// <summary>
    /// Commit changes.
    /// </summary>
    public override void Flush()
    {
      Commit( STGC.STGC_DEFAULT );

      if( m_lockBytes != null )
        m_lockBytes.Flush();
    }

    /// <summary>
    /// Commit changes.
    /// </summary>
    /// <param name="code">Commit code.</param>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When commit operation fails.
    /// </exception>
    public virtual void Commit( STGC code )
    {
      if( ( m_modeStream & STGM.STGM_TRANSACTED ) == STGM.STGM_TRANSACTED )
      {
        CheckStream();

        int error = m_stream.Commit( ( uint )code );

        if( error != 0 )
#if !(WINRT || WP )
          throw new ExternalException( "Commit Operation failed");
#else
                    throw new Exception("Commit Operation failed");
#endif
      }

      m_storage.Commit( ( uint )code );

#if !DOCIO
      m_storage.Commit( ( uint )code );
#endif
    }
    /// <summary>
    /// Discards all changes that have been made to the storage object
    /// since the last commit operation.
    /// </summary>
    public virtual void Revert()
    {
      if( ( m_modeStream & STGM.STGM_TRANSACTED ) != STGM.STGM_TRANSACTED )
        return;

      CheckStream();

      int error = m_stream.Revert();

      if( error != 0 )
#if !(WINRT || WP )
        throw new ExternalException( "Revert Operation failed", error );
#else
        throw new Exception("Revert Operation failed" + error);
#endif
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Seek in stream.
    /// </summary>
    /// <param name="offset">New offset.</param>
    /// <param name="origin">Start point for Seek operation.</param>
    /// <returns>Current position.</returns>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When seek operation fails.
    /// </exception>
    public override long Seek( long offset, SeekOrigin origin )
    {
      CheckStream();

      long lSkip;
      int error = m_stream.Seek( offset, origin, out lSkip );
      if( error != 0 )
#if ( WINRT || WP )
        throw new Exception("Seek Operation failed."+ error );
#else
        throw new ExternalException( "Seek Operation failed.", error );
#endif
      m_lPosition = lSkip;

      return m_lPosition;
    }

    /// <summary>
    /// Set stream length.
    /// </summary>
    /// <param name="value">New stream length</param>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When SetLength operation fails.
    /// </exception>
    public override void SetLength( long value )
    {
      CheckStream();

      int error = m_stream.SetSize( ( ulong )value );
#if !(WINRT || WP )
      if( error != 0 )
        throw new ExternalException( "SetLength Operation failed", error );
#endif
      m_lLength = value;
    }
    #endregion

    #region Class Read/Write Methods
    /// <summary>
    /// Read data from stream.
    /// </summary>
    /// <param name="buffer">Output stream.</param>
    /// <param name="offset">Offset in output buffer.</param>
    /// <param name="count">Quantity of bytes to read.</param>
    /// <returns>Quantity of read bytes.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When buffer is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When offset or count is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When there are not enough items in the buffer.
    /// </exception>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When Read operation fails.
    /// </exception>
    public override int Read( byte[] buffer, int offset, int count )
    {
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );

      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      if( buffer.Length - offset < count )
        throw new ArgumentException( "Invalid offest or count values." );

      CheckStream();

      // If stream reaches the end of file then we optimize.
      if( m_lLength == m_lPosition )
        return 0;

      uint iRead = 0;
      uint iCount = ( uint )count;//Convert.ToUInt32( count );

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Format( "Position: {0}, Size: {1}", m_lPosition, iCount), "Read Data request" );

      byte[] tmpBuff = ( offset > 0 ) ? new byte[ count ] : buffer;
      int error = m_stream.Read( tmpBuff, iCount, ref iRead );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Read Operation failed"+ error);
#else
        throw new ExternalException( "Read Operation failed", error );
#endif
      if( offset > 0 )
      {
        Buffer.BlockCopy( tmpBuff, 0, buffer, offset, ( int )iRead );
      }

      m_lPosition += iRead;

      return ( int )iRead;//Convert.ToInt32( iRead );
    }

    /// <summary>
    /// Write data to stream.
    /// </summary>
    /// <param name="buffer">Buffer with data.</param>
    /// <param name="offset">Offset in input buffer from which data started.</param>
    /// <param name="count">Quantity of bytes which must be written.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When buffer is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When offset or count is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When there are not enough items in the buffer 
    /// or stream is in Read-only mode.
    /// </exception>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When Write operation fails.
    /// </exception>
    public override void Write( byte[] buffer, int offset, int count )
    {
      if( !CanWrite )
        throw new ArgumentException( "Stream in ReadOnly mode. Wrong Operation." );

      // check input parameters
      if( buffer == null )
        throw new ArgumentNullException( "buffer" );

      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );

      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );

      if( buffer.Length - offset < count )
        throw new ArgumentException( "Invalid offest or count values." );

      CheckStream();

      // convert datatypes
      uint iCount = ( uint )count;//Convert.ToUInt32( count );
      uint iWritten = 0;

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Format( "Position: {0}, Size: {1}", m_lPosition, iCount), "Write Data request" );

      // if array data must be stored with offset then create temporary
      // array to which copy data - used to fix Win32 interface Write
      // method limitations
      byte[] tmpBuff = ( offset > 0 ) ? new byte[ count ] : buffer;
      //      if( offset > 0 ) buffer.CopyTo( tmpBuff, offset );

      if( offset > 0 )
        Array.Copy( buffer, offset, tmpBuff, 0, count );

      int error = m_stream.Write( tmpBuff, iCount, ref iWritten );

      if( error != 0 )
#if ( WINRT || WP )
        throw new Exception( "Write Operation failed"+ error );
#else
        throw new ExternalException( "Write Operation failed", error );
#endif
      m_lPosition += iWritten;

      // Update stream length if current position is larger than length.
      if( m_lLength < m_lPosition )
        m_lLength = m_lPosition;
    }
#if DEBUG
    /// <summary>
    ///
    /// </summary>
    public short ReadInt16( int position )
    {
      long lPosition = Position;
      Position = position;
      byte[] arrBuffer = new byte[ 2 ];
      uint uiReadCount = 0;
      m_stream.Read( arrBuffer, 2, ref uiReadCount );

      Position = lPosition;
      return BitConverter.ToInt16( arrBuffer, 0 );
    }
#endif
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent class creation by default constructor.
    /// </summary>
    private StgStream()
    {
    }
    /// <summary>
    /// Open storage/compound file.
    /// </summary>
    /// <param name="fileName">File name of storage.</param>
    /// <param name="flags">Mode which must be used for open operation.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When fileName is NULL.
    /// </exception>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// Couldn't open the storage
    /// </exception>
    public StgStream( string fileName, STGM flags )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );

      int error = API.StgOpenStorage( fileName,
        IntPtr.Zero, flags, IntPtr.Zero, 0, out m_storage );

#if !DOCIO
      if( ( uint )error == ( uint )STG_ERRORS.STG_E_LOCKVIOLATION
        || ( uint )error == ( uint )STG_ERRORS.STG_E_SHAREVIOLATION )
      {
        throw new LockShareViolationException();
      }
#endif

      if( error != 0 )
#if ( WINRT || WP )
     
        throw new Exception( "Cannot open storage. File Name is: " + fileName+ error );
#else
        throw new ExternalException( "Cannot open storage. File Name is: " + fileName, error );
#endif
      // Get names of streams.
      CalculateSubItemsNames();

      m_strFileName = fileName;
      m_modeStorage = flags;
    }
    /// <summary>
    /// Open storage and one stream of it.
    /// </summary>
    /// <param name="fileName">File name of storage.</param>
    /// <param name="storeFlags">Flags that are used for storage open.</param>
    /// <param name="streamName">Stream name.</param>
    /// <param name="streamFlags">Flags which used for stream in storage open.</param>
    public StgStream( string fileName, STGM storeFlags, string streamName, STGM streamFlags )
      : this( fileName, storeFlags )
    {
      OpenStream( streamName, streamFlags );
    }
    /// <summary>
    /// Open storage and its stream in Read-only mode.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="streamName">Stream name.</param>
    public StgStream( string fileName, string streamName )
      : this( fileName, DEF_STORE_READONLY, streamName, DEF_STREAM_READONLY )
    {
    }

    /// <summary>
    /// Open storage in Read-only mode but do not open stream. To open
    /// special stream, use OpenStream methods.
    /// </summary>
    /// <param name="fileName">Storage file name.</param>
    public StgStream( string fileName )
      : this( fileName, DEF_STORE_READONLY )
    {
    }
    /// <summary>
    /// Inherit stream storage and opens its stream in Read-only mode.
    /// </summary>
    /// <param name="storage">Storage of stream.</param>
    /// <param name="streamName">Stream name to open.</param>
    public StgStream( StgStream storage, string streamName )
      : this( storage, streamName, DEF_STREAM_READONLY )
    {
    }
    /// <summary>
    /// Inherit stream storage and opensits streams with the user specified flags.
    /// </summary>
    /// <param name="storage">Storage to inherit.</param>
    /// <param name="streamName">Stream name.</param>
    /// <param name="streamFlags">Stream open flags.</param>
    public StgStream( StgStream storage, string streamName, STGM streamFlags )
      : this( storage, streamName, DEF_STREAM_READONLY, false )
    {
    }
    /// <summary>
    /// Open or create stream specified by user name. 
    /// </summary>
    /// <param name="storage">Inherited storage.</param>
    /// <param name="streamName">Stream name.</param>
    /// <param name="bCreate">True to create stream; otherwise open.</param>
    public StgStream( StgStream storage, string streamName, bool bCreate )
      : this( storage, streamName, ( bCreate ) ? DEF_STREAM_CREATE : DEF_STREAM_READONLY, bCreate )
    {

    }
    /// <summary>
    /// Inherit storage and open or create in it stream with spcified user name
    /// </summary>
    /// <param name="storage">Inherited storage.</param>
    /// <param name="streamName">Stream name.</param>
    /// <param name="streamFlags">Stream open / create flags.</param>
    /// <param name="bCreate">True to create stream; otherwise open.</param>
    public StgStream( StgStream storage, string streamName, STGM streamFlags, bool bCreate )
    {
      if( storage == null )
        throw new ArgumentNullException( "storage" );

      if( storage.m_storage == null )
        throw new ArgumentException( "input storage must be opened" );

      if( streamName == null )
        throw new ArgumentNullException( "streamName" );

      IntPtr comObject =
        Marshal.GetIUnknownForObject( storage.m_storage ); // add ref - need's release

      m_storage = ( IStorage )Marshal.GetObjectForIUnknown( comObject );
      Marshal.Release( comObject );

      m_modeStorage = storage.m_modeStorage;

      m_arrStorages.AddRange( storage.m_arrStorages );
      m_arrStreams.AddRange( storage.m_arrStreams );
      //CalculateSubItemsNames();

      if( bCreate )
      {
        CreateStream( streamName, streamFlags );
      }
      else
      {
        OpenStream( streamName, streamFlags );
      }
    }
    /// <summary>
    /// Create a new instance of StgStream.
    /// </summary>
    /// <param name="stream">Base stream.</param>
    /// <param name="flags">Flags for create stream.</param>
    public StgStream( System.IO.Stream stream, STGM flags )
    {
      if( stream is StgStream )
        throw new ArgumentException( "It is StgStream already." );

      int error = API.CreateILockBytesOnHGlobal( IntPtr.Zero, true, out m_lockBytes );

      if( error != 0 )
#if ( WINRT || WP )

                  throw new Exception("Can't create LockBytes."+ error);
#else
        throw new ExternalException( "Can't create LockBytes.", error );
#endif
      int iSize = ( int )( stream.Length - stream.Position );

      byte[] buffer = new byte[ iSize ];

      stream.Read( buffer, 0, iSize );
      uint uiWritten;

      error = m_lockBytes.WriteAt( 0, buffer, ( uint )buffer.Length,
        out uiWritten );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Can't write LockBytes."+ error);  
#else
        throw new ExternalException( "Can't write LockBytes.", error );
#endif
      error = m_lockBytes.Flush();

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Can't flush LockBytes."+ error);
#else
        throw new ExternalException( "Can't flush LockBytes.", error );
#endif
      //error = API.StgCreateDocfileOnILockBytes( m_lockBytes, flags, 0, out m_storage );
      error = API.StgOpenStorageOnILockBytes( m_lockBytes, null, flags, 0, 0, out m_storage );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Can't open storage on LockBytes."+ error);
#else
        throw new ExternalException( "Can't open storage on LockBytes.", error );
#endif
      // Get names of streams.
      CalculateSubItemsNames();
      m_modeStorage = flags;
    }
    /// <summary>
    /// Create a new instance of StgStream by defoult flag.
    /// </summary>
    /// <param name="stream">Base stream.</param>
    public StgStream( System.IO.Stream stream )
      : this( stream, DEF_STREAM_READONLY )
    {
    }
    /// <summary>
    /// Dispose stream. Close stream, release references on COM interfaces, and
    /// free resources.
    /// </summary>
    new public void Dispose()
    {
      if( !m_bIsDisposed )
      {
        m_bIsDisposed = true;
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Stream Disposed: " + m_strStreamName, "Disposed" );

#if !(WINRT || WP )
        Close();
#endif
        int refCount = Marshal.FinalReleaseComObject( m_storage );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, refCount, "Reference count for RCW:" );

        GC.SuppressFinalize( m_storage );

        if( m_lockBytes != null )
        {
          Marshal.FinalReleaseComObject( m_lockBytes );
          GC.SuppressFinalize( m_lockBytes );
          m_lockBytes = null;
        }

        m_storage = null;
        m_strStorageName = null;
        m_bIsDisposed = true;
        m_arrStorages = null;
        m_arrStreams = null;

        // This is needed to release COM object by the .NET framework GC.
        // Code commented since this causes serious performance overhead
        // GC.Collect(GC.MaxGeneration);
      }
    }
    #endregion

    #region Class Public methods
    /// <summary>
    /// Open stream in Read-only mode.
    /// </summary>
    /// <param name="streamName">Stream name.</param>
    public void OpenStream( string streamName )
    {
      OpenStream( streamName, DEF_STREAM_READONLY );
    }
    /// <summary>
    /// Open stream from storage with specified flags.
    /// </summary>
    /// <param name="streamName">Stream name.</param>
    /// <param name="streamFlags">Stream open flags.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When streamName is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When the specified stream could not be found in the storage.
    /// </exception>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When it was not possible to open the stream.
    /// </exception>
    public void OpenStream( string streamName, STGM streamFlags )
    {
      CheckStorage();

      if( streamName == null )
        throw new ArgumentNullException( "streamName" );

      bool bFound = false;
      for( int i = 0; i < m_arrStreams.Count; i++ )
      {
        string strCurStream = ( string )m_arrStreams[ i ];
#if !(WINRT || WP )
        if( string.Compare( strCurStream, streamName, true ) == 0 )
#else
                if (string.Compare(strCurStream, streamName, StringComparison.CurrentCultureIgnoreCase) == 0)
#endif
        {
          bFound = true;
          streamName = strCurStream;
          break;
        }
      }

      if( !bFound )
        throw new ArgumentException( "In storage cannot be found stream with specified name: " + streamName );
#if !(WINRT || WP )
      // Open stream.
      //m_stream = NULL;
      if( m_stream != null )
        Close();
#endif
      int error = m_storage.OpenStream( streamName, 0, streamFlags, 0, out m_stream );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Cannot open stream."+ error);
#else
        throw new ExternalException( "Cannot open stream.", error );
#endif
#if DEBUG_STGSTREAM && DOCIO
      Debug.WriteLine( streamName, "Open stream" );
#endif

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, streamName, "Open stream" );

      // Get stream length.
      m_lLength = CalculateStreamLength();
      m_lPosition = 0;

      // The flags of the stream calculated here: CanRead, CanWrite, CanSeek, IsTransacted
      m_bCanWrite = ( streamFlags & ( STGM.STGM_WRITE | STGM.STGM_READWRITE ) ) > 0;
      m_bCanSeek = true;
      m_bCanRead = true;
      m_bIsTransacted = ( streamFlags & STGM.STGM_TRANSACTED ) > 0;

      m_strStreamName = streamName;
      m_modeStream = streamFlags;
    }

    /// <summary>
    /// Opens sub storage.
    /// </summary>
    /// <param name="storageName">Storage name to open.</param>
    /// <returns>Returns stream of opened storage.</returns>
    public StgStream OpenSubStorage( string storageName )
    {
      return OpenSubStorage( storageName, DEF_STREAM_READONLY );
    }
    /// <summary>
    /// Opens sub storage.
    /// </summary>
    /// <param name="storageName">Storage name to open.</param>
    /// <param name="flags">Open flags.</param>
    /// <returns>Returns stream of opened storage.</returns>
    public StgStream OpenSubStorage( string storageName, STGM flags )
    {
      if( storageName == null )
        throw new ArgumentNullException( "storageName" );

      CheckStorage();

      IStorage tmpStorage;

      int error = m_storage.OpenStorage( storageName,
        IntPtr.Zero, flags, IntPtr.Zero, 0,
        out tmpStorage );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Cannot open sub storage. sub storage name is: " + storageName+error);
#else
        throw new ExternalException( "Cannot open sub storage. sub storage name is: " + storageName, error );
#endif
#if DEBUG_STGSTREAM && DOCIO
      Debug.WriteLine( storageName, "Open Substorage" );
#endif

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, storageName, "Open Substorage" );

      StgStream output = new StgStream();
      output.m_storage = tmpStorage;
      output.m_strStorageName = storageName;

      // Get stream's names.
      output.CalculateSubItemsNames();
      output.m_modeStorage = flags;

      return output;
    }
    /// <summary>
    /// Creates sub storage.
    /// </summary>
    /// <param name="storageName">Storage name to Create.</param>
    /// <returns>Returns stream of opened storage.</returns>
    public StgStream CreateSubStorage( string storageName )
    {
      return CreateSubStorage( storageName, DEF_STREAM_CREATE );
    }
    /// <summary>
    /// Creates sub storage.
    /// </summary>
    /// <param name="storageName">Storage name for create.</param>
    /// <param name="flags">Create flags.</param>
    /// <returns>Returns stream for created storage.</returns>
    public StgStream CreateSubStorage( string storageName, STGM flags )
    {
      if( storageName == null )
        throw new ArgumentNullException( "storageName" );

#if DEBUG_STGSTREAM && DOCIO
      Debug.WriteLine( storageName, "Create SubStorage" );
#endif

      CheckStorage();

      IStorage tmpStorage;

      int error = m_storage.CreateStorage( storageName, flags, 0, 0, out tmpStorage );

      if( error != 0 )
#if ( WINRT || WP )
        throw new Exception("Cannot open sub storage. sub storage name is: " + storageName+ error);
#else
        throw new ExternalException( "Cannot open sub storage. sub storage name is: " + storageName, error );
#endif
      StgStream output = new StgStream();
      output.m_storage = tmpStorage;
      output.m_strStorageName = storageName;
      tmpStorage = null;

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, storageName, "Create SubStorage" );

      // Get stream's names.
      output.CalculateSubItemsNames();
      output.m_modeStorage = flags;

      m_arrStorages.Add( storageName );

      return output;
    }
    /// <summary>
    /// Create stream in opened storage with specified name.
    /// </summary>
    /// <param name="streamName">Stream name.</param>
    public void CreateStream( string streamName )
    {
      CreateStream( streamName, DEF_STREAM_CREATE );
    }
    /// <summary>
    /// Create stream in storage with specified name and flags.
    /// </summary>
    /// <param name="streamName">Stream name.</param>
    /// <param name="streamFlags">Stream flags.</param>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When its not possible to create stream.
    /// </exception>
    public void CreateStream( string streamName, STGM streamFlags )
    {
      //m_stream = null;
      //if( m_stream == null ) Close();

#if DEBUG_STGSTREAM && DOCIO
      Debug.WriteLine( streamName, "Create Stream" );
#endif

      int error = m_storage.CreateStream( streamName,
        streamFlags, 0, 0, ref m_stream );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Cannot create stream."+error);
#else
        throw new ExternalException( "Cannot create stream.", error );
#endif
      m_strStreamName = streamName;
      m_arrStreams.Add( streamName );

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, streamName, "Create Stream" );

      m_lPosition = 0;
      m_bCanWrite = ( streamFlags & ( STGM.STGM_WRITE | STGM.STGM_READWRITE ) ) > 0;
      m_bCanSeek = true;
      m_bCanRead = true;
      m_bIsTransacted = ( streamFlags & STGM.STGM_TRANSACTED ) > 0;
      CalculateSubItemsNames();
    }
    /// <summary>
    /// Saves internal ILockBytes into stream.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    public void SaveILockBytesIntoStream( System.IO.Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( m_lockBytes == null )
        throw new ArgumentNullException( "m_lockBytes" );

      byte[] buffer = new byte[ DEF_BUFFER_SIZE ];
      uint readBytes;

      for( long position = 0; ; position += DEF_BUFFER_SIZE )
      {
        int result = m_lockBytes.ReadAt( ( ulong )position, buffer, DEF_BUFFER_SIZE, out readBytes );

        if( result != 0 )
#if ( WINRT || WP )
                    throw new Exception("Unable to read bytes from ILockBytes"+ result);
#else
          throw new ExternalException( "Unable to read bytes from ILockBytes", result );
#endif

        stream.Write( buffer, 0, ( int )readBytes );

        if( readBytes < DEF_BUFFER_SIZE )
          break;
      }
    }
    /// <summary>
    /// Searches for stream name in the streams array ignoring case.
    /// </summary>
    /// <param name="strStreamName">Stream name to locate.</param>
    /// <returns>Name of the stream in the storage.</returns>
    public string FindStream( string strStreamName )
    {
      if( strStreamName == null )
        throw new ArgumentNullException( "strStreamName" );

      if( strStreamName.Length == 0 )
        throw new ArgumentException( "strStreamName - string cannot be empty." );

      for( int i = 0, len = m_arrStreams.Count; i < len; i++ )
      {
        string strStream = ( string )m_arrStreams[ i ];
#if !(WINRT || WP )
        if( string.Compare( strStream, strStreamName, true ) == 0 )
#else
                if (string.Compare(strStream, strStreamName, StringComparison.CurrentCultureIgnoreCase) == 0)
#endif
        {
          return strStream;
        }
      }

      return null;
    }

    /// <summary>
    /// Indicates whether storage contains required stream.
    /// </summary>
    /// <param name="strStreamName">Stream to search.</param>
    /// <returns>True if stream was found.</returns>
    public bool ContainsStream( string strStreamName )
    {
      if( strStreamName == null || strStreamName.Length == 0 )
        return false;

      for( int i = 0, len = m_arrStreams.Count; i < len; i++ )
      {
        //string strCurrentStream = m_arrStreams[ i ] as string;
        //object objStream = 

        if( /*strCurrentStream*/m_arrStreams[ i ].Equals( strStreamName ) )
          return true;
      }

      return false;
    }
    /// <summary>
    /// Indicates whether storage contains required substorage.
    /// </summary>
    /// <param name="strStorageName">Storage to search.</param>
    /// <returns>True if stream was found.</returns>
    public bool ContainsStorage( string strStorageName )
    {
      if( strStorageName == null || strStorageName.Length == 0 )
        return false;

      for( int i = 0, len = m_arrStorages.Count; i < len; i++ )
      {
        //string strCurrentStorage = m_arrStorages[ i ] as string;

        if( /*strCurrentStorage*/m_arrStorages[ i ].Equals( strStorageName ) )
          return true;
      }

      return false;
    }
    /// <summary>
    /// Removes the specified storage or stream from this storage object.
    /// </summary>
    /// <param name="elementName">Name of the storage or stream to be removed.</param>
    /// <returns>
    /// 0 - The element was successfully removed.
    /// Otherwise error code.
    /// </returns>
    public int RemoveElement( string elementName )
    {
      return m_storage.DestroyElement( elementName );
    }
    /// <summary>
    /// Copies one storage into another.
    /// </summary>
    /// <param name="source">Source stream.</param>
    /// <param name="destination">Destination stream.</param>
    public static void CopySourceStorages( StgStream source, StgStream destination )
    {
      if( source == null )
        throw new ArgumentNullException( "source" );

      if( destination == null )
        throw new ArgumentNullException( "destination" );

      string name = source.StorageName;

      if( name == null )
        throw new ArgumentException( "Source does not contain opened sub-storage" );

      bool bContains = destination.ContainsStorage( name );

      using( StgStream output = !bContains ?
               destination.CreateSubStorage( name ) :
               destination.OpenSubStorage( name ) )
      {
        source.COMStorage.CopyTo( 0, IntPtr.Zero, IntPtr.Zero, output.COMStorage );
        destination.COMStorage.Commit( 0 );
      }
    }
    #endregion

    #region Class create new Storage
    /// <summary>
    /// Method to create new storage and return StgStream class for it.
    /// </summary>
    /// <param name="fileName">Storage file name.</param>
    /// <returns>Reference on instance which knows how to work with it.</returns>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When compound file couldn't be created.
    /// </exception>
    public static StgStream CreateStorage( string fileName )
    {
      IStorage storage;

      int error = API.StgCreateDocfile( fileName,
        STGM.STGM_CREATE | STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE, 0,
        out storage );

#if !DOCIO
      if( ( uint )error == ( uint )STG_ERRORS.STG_E_LOCKVIOLATION
        || ( uint )error == ( uint )STG_ERRORS.STG_E_SHAREVIOLATION )
      {
        throw new LockShareViolationException();
      }
#endif

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Cannot create compound file."+ error);
#else
        throw new ExternalException( "Cannot create compound file.", error );
#endif
      StgStream stream = new StgStream();
      stream.m_storage = storage;

      return stream;
    }
    /// <summary>
    /// Cretes storage on ILockBytes.
    /// </summary>
    /// <returns>Created storage.</returns>
    static public StgStream CreateStorageOnILockBytes()
    {
      IStorage storage;
      ILockBytes lockBytes;

      int error = API.CreateILockBytesOnHGlobal( IntPtr.Zero, true, out lockBytes );

      if( error != 0 )
#if ( WINRT || WP )
                throw new Exception("Can't create LockBytes." +error);
#else
        throw new ExternalException( "Can't create LockBytes.", error );
#endif
      error = API.StgCreateDocfileOnILockBytes( lockBytes,
        STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE, 0, out storage );

      if( error != 0 )
#if ( WINRT || WP )

        throw new Exception( "Can't create storage on ILockBytes."+ error );
#else
          throw new ExternalException("Can't create storage on ILockBytes.", error);
#endif
      StgStream stream = new StgStream();
      stream.m_storage = storage;
      stream.m_lockBytes = lockBytes;

      return stream;
    }
    #endregion

    #region Class utility methods
    /// <summary>
    /// Check storage availability.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// If storage is NULL.
    /// </exception>
    private void CheckStorage()
    {
      if( m_storage == null )
        throw new ArgumentNullException( "storage", "Storage not initialized" );
    }

    /// <summary>
    /// Check stream availability.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// When stream is NULL.
    /// </exception>
    private void CheckStream()
    {
      if( m_stream == null )
        throw new ArgumentNullException( "Stream", "Stg Stream not initialized" );
    }
    /// <summary>
    /// Calculate stream length.
    /// </summary>
    /// <returns>Returns length of currently open stream.</returns>
    private long CalculateStreamLength()
    {
      CheckStream();

      long lPos = Seek( 0, SeekOrigin.Current );
      long lLen = Seek( 0, SeekOrigin.End );
      Seek( lPos, SeekOrigin.Begin );

      return lLen;
    }

    /// <summary>
    /// Returns list of streams stored in storage.
    /// </summary>
    /// <returns>List of stream names.</returns>
    /// <exception cref="System.Runtime.InteropServices.ExternalException">
    /// When elements of the storage couldn't be enumerated .
    /// </exception>
    /// <exception cref="System.SystemException">
    /// When it is possible to get IEnumSTATSTG interface reference from storage.
    /// </exception>
    private List<string> CalculateStorageStreams()
    {
      List<string> list = new List<string>();
      CalculateSubItems( new SubItemNameEventHandler( ByTypeAccumulate_Streams ), list );
      return list;
    }
    /// <summary>
    /// Return list of sub-storages found in current storage.
    /// </summary>
    /// <returns>List of found storages.</returns>
    private List<string> CalculateStorageSubStorages()
    {
      List<string> list = new List<string>();
      CalculateSubItems( new SubItemNameEventHandler( ByTypeAccumulate_Storages ), list );
      return list;
    }
    /// <summary>
    /// Calculates subItems names.
    /// </summary>
    private void CalculateSubItemsNames()
    {
      m_arrStorages.Clear();
      m_arrStreams.Clear();

      CalculateSubItems( new SubItemNameEventHandler( ByTypeAccumulate_All ), null );
    }
    /// <summary>
    /// Adds data.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="userData">Collection to add.</param>
    private void ByTypeAccumulate_Streams( STATSTG item, object userData )
    {
      if( item.type == STGTY.STGTY_STREAM )
      {
        ( ( List<string> )userData ).Add( item.pwcsName );
      }
    }
    /// <summary>
    /// Adds data as stream type.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="userData">Collection where adding is.</param>
    private void ByTypeAccumulate_Storages( STATSTG item, object userData )
    {
      if( item.type == STGTY.STGTY_STORAGE )
      {
        ( ( List<string> )userData ).Add( item.pwcsName );
      }
    }
    /// <summary>
    /// Adds data as all type.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <param name="userData">Collection where adding is.</param>
    private void ByTypeAccumulate_All( STATSTG item, object userData )
    {
      if( item.type == STGTY.STGTY_STREAM )
      {
        if( m_arrStreams != null )
          m_arrStreams.Add( item.pwcsName );
      }
      else if( item.type == STGTY.STGTY_STORAGE )
      {
        if( m_arrStorages != null )
          m_arrStorages.Add( item.pwcsName );
      }
    }
    /// <summary>
    /// Calculates subItems.
    /// </summary>
    /// <param name="caller">SubItem event handler.</param>
    /// <param name="userData">User data.</param>
    private void CalculateSubItems( SubItemNameEventHandler caller, object userData )
    {
      if( caller == null )
        throw new ArgumentNullException( "caller" );

      CheckStorage();

      const string DEF_FAIL = "Stream Enumeration Operation failed";

      IEnumSTATSTG enm = null;

      int error = m_storage.EnumElements( 0, IntPtr.Zero, 0, ref enm );
#if ( WINRT || WP )
      if( error != 0 )
        throw new Exception( DEF_FAIL+ error );

      if( enm == null )
        throw new Exception( "Cannot get IEnumSTATSTG interface refernce from storage" );
#else
      if( error != 0 )
        throw new ExternalException( DEF_FAIL, error );

      if( enm == null )
        throw new SystemException( "Cannot get IEnumSTATSTG interface refernce from storage" );
#endif
      error = enm.Reset();
      if( error != 0 )
#if ( WINRT || WP )

        throw new Exception( DEF_FAIL+ error );
#else
            throw new ExternalException( DEF_FAIL, error );
#endif
      STATSTG item = new STATSTG();
      uint fetch = 0;

      error = enm.Next( 1, ref item, ref fetch );

      while( 0 == error && 1 == fetch )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "found item with Name : " + item.pwcsName );
        caller( item, userData );
        error = enm.Next( 1, ref item, ref fetch );
      }

      if( error > 1 || error < 0 )
#if ( WINRT || WP )

        throw new Exception( DEF_FAIL+error );
#else
            throw new ExternalException( DEF_FAIL, error );
#endif
      // Release interface.
      Marshal.FinalReleaseComObject( enm );
      enm = null;
    }

    /// <summary>
    /// Delegate that represents subItem name event.
    /// </summary>
    internal delegate void SubItemNameEventHandler( STATSTG item, object userData );
    #endregion
  }
}