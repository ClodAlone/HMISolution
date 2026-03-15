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

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
    /// <summary>
    /// This is wrapper over compound stream object. Simply redirects all calls to it
    /// with one exception - it doesn't dispose underlying stream object.
    /// </summary>
    class CompoundStreamWrapper : CompoundStream
    {
        #region Members
        /// <summary>
        /// Wrapped stream object.
        /// </summary>
        private CompoundStream m_stream;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return m_stream.CanRead;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return m_stream.CanSeek;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return m_stream.CanWrite;
            }
        }
        /// <summary>
        /// Gets the length in bytes of the stream. Read-only.
        /// </summary>
        public override long Length
        {
            get
            {
                return m_stream.Length;
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
        #endregion

        #region Methods
        /// <summary>
        /// Initializes new instance of the wrapper.
        /// </summary>
        /// <param name="wrapped">Object to wrap.</param>
        public CompoundStreamWrapper(CompoundStream wrapped)
            :
          base(wrapped.Name)
        {
            m_stream = wrapped;
        }
        /// <summary>
        /// Causes any buffered data to be written to the underlying compound file.
        /// </summary>
        public override void Flush()
        {
            m_stream.Flush();
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
        /// <returns>The total number of bytes read into the buffer. This can be less than
        /// the number of bytes requested if that many bytes are not currently available,
        /// or zero (0) if the end of the stream has been reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_stream.Read(buffer, offset, count);
        }
        /// <summary>
        /// Sets the position within the current stream. 
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference
        /// point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return m_stream.Seek(offset, origin);
        }
        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            m_stream.SetLength(value);
        }
        /// <summary>
        /// writes a sequence of bytes to the current stream and advances the current
        /// position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes
        /// from buffer to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to
        /// begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            m_stream.Write(buffer, offset, count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (m_stream != null)
            {
                base.Dispose(disposing);

                // NOTE: we don't dispose wrapped item. This should be done by some other object.
                m_stream = null;
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
