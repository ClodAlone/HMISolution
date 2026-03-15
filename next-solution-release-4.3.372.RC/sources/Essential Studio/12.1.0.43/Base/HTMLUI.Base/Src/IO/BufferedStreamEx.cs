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
using System.Collections;
using System.IO;
using System.Diagnostics;
#endregion

namespace Syncfusion.HTMLUI.Base
{
  /// <summary>
  /// Special wrapper which allows users to control access to stream
  /// and cache data. Our implementation fixes the bug of .NET BufferedStream
  /// class: on setting Position property, class resets internal cache.
  /// Our class detects situations when reset of cache is not needed.
  /// </summary>
  public sealed class BufferedStreamEx : System.IO.Stream
  {
    #region Class constants
    /// <summary>
    /// Default buffer allocation size.
    /// </summary>
    private const int _DefaultBufferSize = 8 * 1024;
    #endregion

    #region Class members
    /// <summary>
    /// Underlying stream. Close sets _s to NULL.
    /// </summary>
    private System.IO.Stream  _s;
    /// <summary>
    /// Shared read / write buffer. Alloc on first use.
    /// </summary>
    private byte[]  _buffer;
    /// <summary>
    /// Reads pointer within shared buffer.
    /// </summary>
    private int     _readPos;
    /// <summary>
    /// Number of bytes read in buffer from _s.
    /// </summary>
    private int     _readLen;
    /// <summary>
    /// Writes pointer within shared buffer.
    /// </summary>
    private int     _writePos;
    /// <summary>
    /// Length of internal buffer, if its allocated.
    /// </summary>
    private int     _bufferSize;
    /// <summary>
    /// Stream position on moment of buffer read.
    /// </summary>
    private long    _streamPos;
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether data can be read from wrapped stream.
    /// True if data can be read.
    /// </summary>
    /// <exception cref="ArgumentNullException">Class instance was disposed.</exception>
    public override bool CanRead
    {
      get
      {
        if( _s == null )
          throw new ArgumentNullException( "stream" );

        return _s.CanRead;
      }
    }
    /// <summary>
    /// Indicates whether data can be written to wrapped stream.
    /// </summary>
    /// <exception cref="ArgumentNullException">Class instance was disposed.</exception>
    public override bool CanWrite
    {
      get
      {
        if( _s == null )
          throw new ArgumentNullException( "stream" );

        return _s.CanWrite;
      }
    }
    /// <summary>
    /// Indicates whether wrapped stream supports seek operations.
    /// </summary>
    /// <exception cref="ArgumentNullException">Class instance was disposed.</exception>
    public override bool CanSeek
    {
      get
      {
        if( _s == null )
          throw new ArgumentNullException( "stream" );

        return _s.CanSeek;
      }
    }
    /// <summary>
    /// Returns the length of wrapped stream. On property value get class Flush not
    /// written to stream data.
    /// </summary>
    /// <exception cref="ArgumentNullException">Class instance was disposed.</exception>
    public override long Length
    {
      get
      {
        if( _s == null )
          throw new ArgumentNullException( "stream" );

        if( _writePos > 0 ) FlushWrite();

        return _s.Length;
      }
    }

    /// <summary>
    /// Returns current position of stream. Position can be different when wrapped
    /// stream, because wrapped stream will point on last byte of
    /// cached data. On property set not written data flushed to stream.
    /// </summary>
    /// <exception cref="ArgumentNullException">Class instance was disposed.</exception>
    /// <exception cref="ArgumentException">Wrapped stream does not support seek operation.</exception>
    public override long Position
    {
      get
      {
        if( _s == null )
          throw new ArgumentNullException( "stream" );

        if( !_s.CanSeek )
          throw new ArgumentException( "Stream does not support seek operation." );

        // At the same time readPos and writePos can be larger then zero.
        return _streamPos + _readPos + _writePos;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "value" );
        if( _s == null )
          throw new ArgumentNullException( "stream" );
        if( !_s.CanSeek )
          throw new ArgumentException( "Stream does not support seek operation." );

        if( _writePos > 0 ) FlushWrite();

        // If new position does not limit our internal buffer then
        // simply modify internal position variables, without stream calls.
        if( _streamPos + _readLen > value && value >= _streamPos )
        {
          _readPos = (int)(value - _streamPos);
        }
        else
        {
          _readPos = 0;
          _readLen = 0;

          _streamPos = _s.Seek( value, SeekOrigin.Begin );
        }
      }
    }
    /// <summary>
    /// Returns the reference on stream wrapped by BufferStreamEx.
    /// </summary>
    public System.IO.Stream BaseStream
    {
      get
      {
        return _s;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor. Hidden to class users.
    /// </summary>
    private BufferedStreamEx()
    {
    }

    /// <summary>
    /// Overloaded constructor. Initializes class by an instance of stream and DefaultBufferSize == 4096.
    /// </summary>
    /// <param name="stream">Stream which our class must wrap.</param>
    public BufferedStreamEx( System.IO.Stream stream )
      : this( stream, _DefaultBufferSize )
    {
    }

    /// <summary>
    /// Initializes class by an instance of stream and user defined cache size.
    /// </summary>
    /// <param name="stream">Stream which our class must wrap.</param>
    /// <param name="bufferSize">User defined cache size.</param>
    /// <exception cref="ArgumentException">Stream does not support Read and Write operations.</exception>
    /// <exception cref="ArgumentOutOfRangeException">BufferSize is equal to or less than zero.</exception>
    public BufferedStreamEx( System.IO.Stream stream, int bufferSize )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( bufferSize <= 0 )
        throw new ArgumentOutOfRangeException( "bufferSize" );

      //Debug.Assert( !(stream is FileStream), "FileStream is buffered - don't wrap it in a BufferedStreamEx." );
      Debug.Assert( !(stream is MemoryStream), "MemoryStream shouldn't be wrapped in a BufferedStreamEx." );

      _s = stream;
      _bufferSize = bufferSize;

      // Allocate _buffer on its first use - it will not be used if all reads
      // & writes are greater than or equal to buffer size.
      if( !_s.CanRead && !_s.CanWrite )
        throw new ArgumentException( "stream read / write operations is closed.", "stream" );
    }
    #endregion

    #region Read operations
    /// <summary>
    /// Reads data from stream. If data is cached, then class will not call wrapped
    /// stream and simply return copy of cached data.
    /// </summary>
    /// <param name="array">Output buffer.</param>
    /// <param name="offset">Offset in output buffer where data from stream must be placed.</param>
    /// <param name="count">Count of bytes which class must return.</param>
    /// <returns>Quantity of read bytes.</returns>
    public override int Read( byte[] array, int offset, int count )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );
      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );
      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );
      if( array.Length - offset < count )
        throw new ArgumentException();
      if( _s == null )
        throw new ArgumentException( "stream" );

      // Size of free space in buffer.
      int n = _readLen - _readPos;

      // If the read buffer is empty, read into either user's array or our
      // buffer, depending on number of bytes user asked for and buffer size.
      if( n == 0 )
      {
        if( !_s.CanRead )
          throw new ArgumentException( "Stream does not support read operation." );

        if( _writePos > 0 ) FlushWrite();

        // If user tries to read from stream buffer that is more then our internal
        // buffer can store then redirect call to stream.
        if( count >= _bufferSize )
        {
          n = _s.Read( array, offset, count );

          // We must reset internal buffer because stream position after read changed.
          _streamPos = _s.Position;
          _readPos = 0;
          _readLen = 0;
          return n;
        }

        // Read data into internal buffer.
        if( _buffer == null ) _buffer = new byte[ _bufferSize ];

        long oldStreamPos = _streamPos;
        _streamPos = _s.Position;
        n = _s.Read( _buffer, 0, _bufferSize );
        if( n == 0 )
        {
          // If stream does not have more data then _streamPosition value must
          // restored to initial state.
          _streamPos = oldStreamPos;
          return 0;
        }

        _readPos = 0;
        _readLen = n;
      }

      // Now copy min of count or numBytesAvailable (ie, near EOF) to array.
      if( n > count ) n = count;
      Buffer.BlockCopy( _buffer, _readPos, array, offset, n );
      _readPos += n;

      // If we hit the end of the buffer and don't have enough bytes, we must
      // read some more from the underlying stream.
      if( n < count )
      {
        int moreBytesRead = _s.Read( array, offset + n, count - n );
        n += moreBytesRead;

        // Reset buffer if our internal storage does not hold data from stream now.
        _streamPos = _s.Position;
        _readPos = 0;
        _readLen = 0;
      }

      return n;
    }
    /// <summary>
    /// Reads a byte from the underlying stream. Returns the byte cast to an integer;
    /// -1 if reading from the end of the stream.
    /// </summary>
    /// <returns>Read byte.</returns>
    public override int ReadByte()
    {
      if( _s == null )
        throw new ArgumentNullException( "stream" );
      if( _readLen == 0 && !_s.CanRead )
        throw new ArgumentException( "Stream does not support read operation." );

      if( _readPos == _readLen )
      {
        if( _writePos > 0 ) FlushWrite();
        if( _buffer == null ) _buffer = new byte[ _bufferSize ];
        _streamPos = _s.Position;
        _readLen = _s.Read( _buffer, 0, _bufferSize );
        _readPos = 0;
      }

      if( _readPos == _readLen ) return -1;

      return _buffer[ _readPos++ ];
    }
    #endregion

    #region Write operations
    /// <summary>
    /// Writes portion of data into wrapped stream. If data can be placed into
    /// cache, then it will be there and only on flush operation data will be
    /// saved to wrapped stream.
    /// </summary>
    /// <param name="array">Array containing data.</param>
    /// <param name="offset">Offset to the beginning of the portion of data.</param>
    /// <param name="count">Number of bytes in the portion of data.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If array or stream is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If offset or count is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// If array does not contain required elements count.
    /// </exception>
    public override void Write( byte[] array, int offset, int count )
    {
      if( array == null )
        throw new ArgumentNullException( "array" );
      if( offset < 0 )
        throw new ArgumentOutOfRangeException( "offset" );
      if( count < 0 )
        throw new ArgumentOutOfRangeException( "count" );
      if( array.Length - offset < count )
        throw new ArgumentException();
      if( _s == null )
        throw new ArgumentNullException( "stream" );

      if( _writePos == 0 )
      {
        // Ensure we can write to the stream and ready buffer for writing.
        if( !_s.CanWrite )
          throw new ArgumentException( "Stream does not support write operation." );

        if( _readPos < _readLen ) FlushRead();
      }

      // If our buffer has data in it, copy data from the user's array into
      // the buffer, and if we can fit it all there, return. Otherwise, write
      // the buffer to disk and copy any remaining data into our buffer.
      // The assumption here is memcpy is cheaper than disk (or net) IO.
      // (10 milliseconds to disk vs. ~20-30 microseconds for a 4K memcpy)
      // So the extra copying will reduce the total number of writes, in
      // non-pathological cases (ie, write 1 byte, then write for the buffer
      // size repeatedly).
      if( _writePos > 0 )
      {
        int numBytes = _bufferSize - _writePos;   // space left in buffer

        if( numBytes > 0 )
        {
          if( numBytes > count ) numBytes = count;
          Buffer.BlockCopy( array, offset, _buffer, _writePos, numBytes );
          _writePos += numBytes;
          if( count == numBytes ) return;

          // If we have less bytes in buffer if needed for operation.
          offset += numBytes;
          count -= numBytes;
        }

        // Reset our buffer. We essentially want to call FlushWrite
        // without calling flush on the underlying stream.
        _s.Write( _buffer, 0, _writePos );
        _streamPos = _s.Position;
        _writePos = 0;
      }

      // If the buffer would slow writes down, avoid buffer completely.
      if( count >= _bufferSize )
      {
        _s.Write( array, offset, count );
        _streamPos = _s.Position;
        return;
      }
      else if( count == 0 ) return;  // Don't allocate a buffer then call memcpy for 0 bytes.

      if( _buffer == null ) _buffer = new byte[ _bufferSize ];

      // Copy remaining bytes into buffer to write at a later date.
      Buffer.BlockCopy( array, offset, _buffer, 0, count );
      _writePos += count;
      //_streamPos = _s.Position + count;
    }
    /// <summary>
    /// Writes byte of information into stream.
    /// </summary>
    /// <param name="value">Value which must be written.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// If can't write into stream.
    /// </exception>
    public override void WriteByte( byte value )
    {
      if( _s == null )
        throw new ArgumentNullException( "stream" );

      if( _writePos == 0 )
      {
        if( !_s.CanWrite )
          throw new ArgumentException( "Stream does not support write operation." );

        if( _readPos < _readLen ) FlushRead();

        if( _buffer == null ) _buffer = new byte[ _bufferSize ];
      }

      if( _writePos == _bufferSize ) FlushWrite();

      _buffer[ _writePos++ ] = value;
    }
    #endregion

    #region Seek operation
    /// <summary>
    /// Seek operation. On any seek operation not written, data from cache will be
    /// flushed to wrapped stream. It is better to use position property if optimized for
    /// cache use.
    /// </summary>
    /// <param name="offset">New offset of stream.</param>
    /// <param name="origin">Start point of seek operation.</param>
    /// <returns>Current position.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// If stream is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// If can't seek in the stream.
    /// </exception>
    public override long Seek( long offset, SeekOrigin origin )
    {
      if( _s == null )
        throw new ArgumentNullException( "stream" );

      if( !_s.CanSeek )
        throw new ArgumentException( "Stream does not support seek operation." );

      // If we've got bytes in our buffer to write, write them out.
      // If we've read in and consumed some bytes, we'll have to adjust
      // our seek positions ONLY IF we're seeking relative to the current
      // position in the stream.
      Debug.Assert( _readPos <= _readLen, "_readPos <= _readLen" );

      if( _writePos > 0 )
      {
        FlushWrite();
      }
      else if( origin == SeekOrigin.Current )
      {
        // Don't call FlushRead here, which would cause an infinite
        // loop. Simply adjust the seek origin. This isn't necessary
        // if we're seeking relative to the beginning or end of the stream.
        Debug.Assert( _readLen - _readPos >= 0,
          "_readLen (" + _readLen + ") - _readPos (" + _readPos + ") >= 0" );

        offset -= (_readLen - _readPos);
      }

      long oldPos = _s.Position + ( _readPos - _readLen );
      long pos = _s.Seek( offset, origin );

      // We now must update the read buffer. We can in some cases simply
      // update _readPos within the buffer, copy around the buffer so our
      // position property is still correct, and avoid having to do more
      // reads from the disk. Otherwise, discard the buffer's contents.
      if( _readLen > 0 )
      {
        // We can optimize the following condition:
        // oldPos - _readPos <= pos < oldPos + _readLen - _readPos
        if( oldPos == pos )
        {
          if( _readPos > 0 )
          {
            Buffer.BlockCopy( _buffer, _readPos, _buffer, 0, _readLen - _readPos );
            _readLen -= _readPos;
            _readPos = 0;
          }

          // If we still have buffered data, we must update the stream's
          // position so our position property is correct.
          if( _readLen > 0 )
            _s.Seek(_readLen, SeekOrigin.Current);
        }
        else if( oldPos - _readPos < pos && pos < oldPos + _readLen - _readPos )
        {
          int diff = (int)( pos - oldPos );
          Buffer.BlockCopy( _buffer, _readPos+diff, _buffer, 0, _readLen - ( _readPos + diff ) );
          _readLen -= ( _readPos + diff );
          _readPos = 0;

          if( _readLen > 0 ) _s.Seek( _readLen, SeekOrigin.Current );
        }
        else
        {
          // Lose the read buffer.
          _readPos = 0;
          _readLen = 0;
        }

        Debug.Assert( _readLen >= 0 && _readPos <= _readLen, "_readLen should be nonnegative, and _readPos should be less than or equal _readLen" );
        Debug.Assert( pos == Position, "Seek optimization: pos != Position!  Buffer math was mangled." );
      }

      _streamPos = pos;
      return pos;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Saves all data from cache and closes stream.
    /// </summary>
    public override void Close()
    {
      if( _s != null )
      {
        Flush();
        _s.Close();
      }

      _s = null;
      _buffer = null;
    }

    /// <summary>
    /// Flushes data. Resets cache.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">
    /// When stream is NULL.
    /// </exception>
    public override void Flush()
    {
      if( _s == null )
        throw new ArgumentNullException( "stream" );

      if( _writePos > 0 )
      {
        FlushWrite();
      }
      else if( _readPos < _readLen && _s.CanSeek )
      {
        FlushRead();
      }
    }
    /// <summary>
    /// Reading is done by blocks from the file, but someone could read
    /// one byte from the buffer then write. At that point, the OS's file
    /// pointer is out of sync with the stream's position. All write
    /// functions should call this function to preserve the position in the file.
    /// </summary>
    private void FlushRead()
    {
      if( _readPos - _readLen != 0 )
      {
        _s.Seek( _readPos - _readLen, SeekOrigin.Current );
      }

      _readPos = 0;
      _readLen = 0;
    }
    /// <summary>
    /// Writes are buffered. Anytime the buffer fills up
    /// (_writePos + delta > _bufferSize) or the buffer switches to reading
    /// and there is dirty data (_writePos > 0), this function must be called.
    /// </summary>
    private void FlushWrite()
    {
      _s.Write( _buffer, 0, _writePos );
      _writePos = 0;
      _streamPos = _s.Position;
      _s.Flush();
    }
    /// <summary>
    /// Sets the length of the wrapped stream.
    /// </summary>
    /// <param name="value">New length of stream.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// When value is less than zero.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// When stream is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// If can't seek or can't write into stream.
    /// </exception>
    public override void SetLength( long value )
    {
      if( value < 0 )
        throw new ArgumentOutOfRangeException( "value" );
      if( _s == null )
        throw new ArgumentNullException( "stream" );
      if( !_s.CanSeek )
        throw new ArgumentException( "Stream does not support seek operation." );
      if( !_s.CanWrite )
        throw new ArgumentException( "Stream does not support write operation." );

      if( _writePos > 0 )
      {
        FlushWrite();
      }
      else if( _readPos < _readLen )
      {
        FlushRead();
      }

      _s.SetLength( value );
    }
    #endregion
  }
}
