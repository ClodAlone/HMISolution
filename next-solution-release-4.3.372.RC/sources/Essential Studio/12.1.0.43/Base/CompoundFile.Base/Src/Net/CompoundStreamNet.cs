#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

#if DOCIO
using Syncfusion.CompoundFile.DocIO;
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
  /// <summary>
  /// .Net implementation of the compound stream.
  /// </summary>
  class CompoundStreamNet :
    CompoundStream,
    ICompoundItem
  {
    #region Members
    /// <summary>
    /// Parent file item.
    /// </summary>
    private CompoundFile m_parentFile;
    /// <summary>
    /// Directory entry of this stream.
    /// </summary>
    private DirectoryEntry m_entry;
    /// <summary>
    /// Stream with data. If it is null, then data hasn't been read yet or stream is closed.
    /// </summary>
    private Stream m_stream;
    #endregion

    #region Properties
    /// <summary>
    /// Returns directory entry for this stream.
    /// </summary>
    public DirectoryEntry Entry
    {
      get
      {
        return m_entry;
      }
    }
    protected Stream Stream
    {
      get
      {
        return m_stream;
      }
      set
      {
        m_stream = value;
      }
    }
    public CompoundFile ParentFile
    {
      get
      {
        return m_parentFile;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the stream.
    /// </summary>
    /// <param name="file">Parent file object.</param>
    /// <param name="entry">Entry that describes this stream item.</param>
    public CompoundStreamNet( CompoundFile file, DirectoryEntry entry )
      : base( entry.Name )
    {
      if( file == null )
        throw new ArgumentNullException( "file" );

      if( entry == null )
        throw new ArgumentNullException( "entry" );

      if( entry.Type != DirectoryEntry.EntryType.Stream )
        throw new ArgumentOutOfRangeException( "entry" );

      m_parentFile = file;
      m_entry = entry;
    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void Open()
    {
      if( m_stream == null )
        m_stream = m_parentFile.GetEntryStream( m_entry );
    }
    #endregion

    #region CompoundStream Members
    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position
    /// within the stream by the number of bytes read. 
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer
    /// contains the specified byte array with the values between offset and
    /// (offset + count - 1) replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin
    /// storing the data read from the current stream.</param>
    /// <param name="length">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>The total number of bytes read into the buffer. This can be less than
    /// the number of bytes requested if that many bytes are not currently available,
    /// or zero (0) if the end of the stream has been reached.</returns>
    public override int Read( byte[] buffer, int offset, int length )
    {
      return m_stream.Read( buffer, offset, length );
    }
    /// <summary>
    /// writes a sequence of bytes to the current stream and advances the current
    /// position within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">An array of bytes. This method copies count bytes
    /// from buffer to the current stream.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to
    /// begin copying bytes to the current stream.</param>
    /// <param name="length">The number of bytes to be written to the current stream.</param>
    public override void Write( byte[] buffer, int offset, int length )
    {
      m_stream.Write( buffer, offset, length );
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
      return m_stream.Seek( offset, origin );
    }
    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="value">The desired length of the current stream in bytes.</param>
    public override void SetLength( long value )
    {
      m_stream.SetLength( value );
    }
#if ( WINRT )
    public void Dispose()
#else
    public override void Close()
#endif
    {
      Flush();
      m_stream.Dispose();
      m_stream = null;
    }
    /// <summary>
    /// Gets the length in bytes of the stream. Read-only.
    /// </summary>
    public override long Length
    {
      get
      {
        return ( m_stream != null ) ? m_stream.Length : m_entry.Size;
      }
    }
    /// <summary>
    /// Gets or sets the position within the current stream.
    /// </summary>
    public override long Position
    {
      get
      {
        return m_stream.Position;
      }
      set
      {
        m_stream.Position = value;
      }
    }
    /// <summary>
    /// Causes any buffered data to be written to the underlying compound file.
    /// </summary>
    public override void Flush()
    {
      // Here we have to go to parent element and ask him to write this data inside.
      if( m_stream != null )
        m_parentFile.SetEntryStream( m_entry, m_stream );
    }
    /// <summary>
    /// Gets a value indicating whether the current stream supports reading.
    /// </summary>
    public override bool CanRead
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking.
    /// </summary>
    public override bool CanSeek
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Gets a value indicating whether the current stream supports writing.
    /// </summary>
    public override bool CanWrite
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Releases the unmanaged resources used by the Stream and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources;
    /// false to release only unmanaged resources.</param>
    protected override void Dispose( bool disposing )
    {
      if( m_stream != null )
      {
        base.Dispose( disposing );
        m_stream.Dispose();
        m_stream = null;
        m_parentFile = null;
        m_entry = null;
        GC.SuppressFinalize( this );
      }
    }
    #endregion
  }
}
