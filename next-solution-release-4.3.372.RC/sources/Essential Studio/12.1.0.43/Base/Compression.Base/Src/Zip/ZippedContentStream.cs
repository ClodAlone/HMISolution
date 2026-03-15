#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

#if !(SILVERLIGHT || WP)
using System.IO.Compression;
#endif

using System.Collections.Generic;
using System.Text;
using Syncfusion.Compression.Zip;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 || (SILVERLIGHT) )
namespace Syncfusion.Compression.Zip
{
  /// <summary>
  /// 
  /// </summary>
  public class ZippedContentStream : Stream
  {
    #region Members
    private MemoryStream m_stream = new MemoryStream();
    /// <summary>
    /// 
    /// </summary>
    private /*DeflateStream*/Stream m_deflateStream;
    /// <summary>
    /// 
    /// </summary>
    private uint m_uiCrc32;
    /// <summary>
    /// 
    /// </summary>
    private long m_lSize;
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating whether the current stream supports reading. Read-only.
    /// </summary>
    public override bool CanRead
    {
      get
      {
        return m_deflateStream.CanRead;
      }
    }
    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking. Read-only.
    /// </summary>
    public override bool CanSeek
    {
      get
      {
        return m_deflateStream.CanSeek;
      }
    }
    /// <summary>
    /// Gets a value indicating whether the current stream supports writing. Read-only.
    /// </summary>
    public override bool CanWrite
    {
      get
      {
        return m_deflateStream.CanWrite;
      }
    }
    /// <summary>
    /// Gets the length in bytes of the stream. Read-only.
    /// </summary>
    public override long Length
    {
      get
      {
        return m_deflateStream.Length;
      }
    }
    /// <summary>
    /// Gets or sets the position within the current stream. Read-only.
    /// </summary>
    public override long Position
    {
      get
      {
        return m_deflateStream.Position;
      }
      set
      {
        throw new Exception( "The method or operation is not implemented." );
      }
    }
    /// <summary>
    /// This property returns stream with zipped content. It closes internal deflate
    /// stream, so you won't be able to write anything in int. Read-only.
    /// </summary>
    public Stream ZippedContent
    {
      get
      {
        m_deflateStream.Close();
        return m_stream;
      }
    }
    /// <summary>
    /// Returns computed crc32 value. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public uint Crc32
    {
      get
      {
        return m_uiCrc32;
      }
    }
    /// <summary>
    /// Returns size of the unzipped data. Read-only.
    /// </summary>
    public long UnzippedSize
    {
      get
      {
        return m_lSize;
      }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes new instance of the stream.
    /// </summary>
    private ZippedContentStream()
    {
      m_deflateStream = CreateDeflateStream( m_stream );
    }
    /// <summary>
    /// Initializes new instance of the stream.
    /// </summary>
    public ZippedContentStream( ZipArchive.CompressorCreator createCompressor )
    {
      m_deflateStream = createCompressor( m_stream );
    }
    #endregion

    #region Methods
    private Stream CreateDeflateStream(Stream stream)
    {
      return 
        new NetCompressor( CompressionLevel.Best, stream );
        //new DeflateStream( m_stream, CompressionMode.Compress, true );
    }
    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    public override void Flush()
    {
      m_deflateStream.Flush();
    }
    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position
    /// within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer
    /// contains the specified byte array with the values between offset and
    /// (offset + count - 1) replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin
    /// storing the data read from the current stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>The total number of bytes read into the buffer. This can be less
    /// than the number of bytes requested if that many bytes are not currently
    /// available, or zero (0) if the end of the stream has been reached.</returns>
    public override int Read( byte[] buffer, int offset, int count )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <param name="origin">A value of type SeekOrigin indicating the reference
    /// point used to obtain the new position.</param>
    /// <returns>The new position within the current stream.</returns>
    public override long Seek( long offset, SeekOrigin origin )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="value">The desired length of the current stream in bytes.</param>
    public override void SetLength( long value )
    {
      throw new Exception( "The method or operation is not implemented." );
    }
    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current
    /// position within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">An array of bytes. This method copies count bytes
    /// from buffer to the current stream.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin
    /// copying bytes to the current stream. </param>
    /// <param name="count">The number of bytes to be written to the current stream.</param>
    public override void Write( byte[] buffer, int offset, int count )
    {
      m_deflateStream.Write( buffer, offset, count );
      m_uiCrc32 = ZipCrc32.ComputeCrc( buffer, offset, count, m_uiCrc32 );
      m_lSize += count;
    }
    #endregion
  }
}
#endif

