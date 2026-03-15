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
using Syncfusion.CompoundFile;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Native
#else
namespace Syncfusion.CompoundFile.XlsIO.Native
#endif
{
  /// <summary>
  /// Implementation of compound stream based on standard COM object.
  /// </summary>
  class NativeStream : CompoundStream
  {
    #region Members
    /// <summary>
    /// 
    /// </summary>
    private IStream m_stream;
    /// <summary>
    /// Stream position.
    /// </summary>
    private long m_lPosition;

    private static bool m_bShown;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the stream object.
    /// </summary>
    /// <param name="stream">COM stream to use.</param>
    /// <param name="name">Name of the stream.</param>
        public NativeStream(IStream stream, string name)
            : base(name)
    {
            if (stream == null)
                throw new ArgumentNullException("stream");

      m_stream = stream;
    }
    #endregion

    #region CompoundStream Members
    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position
    /// within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains
    /// the specified byte array with the values between offset and (offset + count - 1)
    /// replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin storing
    /// the data read from the current stream.</param>
    /// <param name="length">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>The total number of bytes read into the buffer. This can be less
    /// than the number of bytes requested if that many bytes are not currently
    /// available, or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int length)
    {
            CheckBufferOffsetLength(buffer, offset, length);

            byte[] arrBuffer = (offset != 0) ?
              new byte[length] :
        buffer;

      uint uiReadCount = 0;
            m_stream.Read(arrBuffer, (uint)length, ref uiReadCount);

            int iCount = (int)uiReadCount;
      m_lPosition += uiReadCount;

            if (offset != 0)
                Buffer.BlockCopy(arrBuffer, 0, buffer, offset, iCount);

      return iCount;
    }
    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position
    /// within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">An array of bytes. This method copies length bytes from buffer to the current stream.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
    /// <param name="length">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int length)
    {
            CheckBufferOffsetLength(buffer, offset, length);

      byte[] arrBufferToWrite;

            if (offset == 0)
      {
        arrBufferToWrite = buffer;
      }
      else
      {
                arrBufferToWrite = new byte[length];
                Buffer.BlockCopy(buffer, offset, arrBufferToWrite, 0, length);
      }

      uint uiWritten = 0;
            m_stream.Write(arrBufferToWrite, (uint)length, ref uiWritten);
      m_lPosition += uiWritten;
    }
    /// <summary>
    /// Sets the position within the current stream. 
    /// </summary>
    /// <param name="position">A byte offset relative to the origin parameter.</param>
    /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
    /// <returns>The new position within the current stream.</returns>
        public override long Seek(long position, System.IO.SeekOrigin origin)
    {
      long lNewPosition;
            m_stream.Seek(position, origin, out lNewPosition);

      m_lPosition = lNewPosition;
      return lNewPosition;
    }
    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="length">The desired length of the current stream in bytes.</param>
        public override void SetLength(long length)
    {
            m_stream.SetSize((ulong)length);
    }
    /// <summary>
    /// Gets the length in bytes of the stream.
    /// </summary>
    public override long Length
    {
      get
      {
                if (!m_bShown)
        {
                    //Debug.Fail("Optimize Length getter");
          m_bShown = true;
        }

        long lResult;
                m_stream.Seek(0, SeekOrigin.End, out lResult);
                m_stream.Seek(m_lPosition, SeekOrigin.Begin, out m_lPosition);

        return lResult;
      }
    }
    /// <summary>
    /// Gets or sets the position within the current stream.
    /// </summary>
    public override long Position
    {
      get
      {
        return m_lPosition;
      }
      set
      {
                m_lPosition = Seek(value, SeekOrigin.Begin);
      }
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
    /// Clears all buffers for this stream and causes any buffered data to be
    /// written to the underlying device.
    /// </summary>
    public override void Flush()
    {
            m_stream.Commit(0);
    }
    /// <summary>
    /// Releases the unmanaged resources used by the Stream and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources;
    /// false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
    {
            base.Dispose(disposing);

            m_stream.Commit(0);
            Marshal.FinalReleaseComObject(m_stream);
      m_stream = null;
      m_lPosition = -1;
    }
    /// <summary>
    /// Checks whether offset and length can be fit inside specified buffer.
    /// </summary>
    /// <param name="buffer">Buffer to check.</param>
    /// <param name="offset">Offset to check.</param>
    /// <param name="length">Length to check.</param>
        private void CheckBufferOffsetLength(byte[] buffer, int offset, int length)
    {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (offset + length > buffer.Length)
                throw new ArgumentOutOfRangeException("Array size, offset and length doesn't match each other");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (length < 0)
                throw new ArgumentOutOfRangeException("length");
    }
    #endregion
  }
}
