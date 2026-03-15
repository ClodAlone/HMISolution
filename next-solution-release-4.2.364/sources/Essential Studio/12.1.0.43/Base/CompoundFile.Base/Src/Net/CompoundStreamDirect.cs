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
    class CompoundStreamDirect :
      CompoundStreamNet,
      ICompoundItem
    {
        #region Constants
        private const int MinimumSize = 32768;
        #endregion

        #region Members
        /// <summary>
        /// Stream position.
        /// </summary>
        private long m_lPosition;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes new instance of the stream.
        /// </summary>
        /// <param name="file">Parent file object.</param>
        /// <param name="entry">Entry that describes this stream item.</param>
        public CompoundStreamDirect(CompoundFile file, DirectoryEntry entry)
            : base(file, entry)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Open()
        {
            if (Entry.Size < 32768)
                base.Open();

            m_lPosition = 0;
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
        public override int Read(byte[] buffer, int offset, int length)
        {
            int result;

            if (Stream != null)
            {
                result = base.Read(buffer, offset, length);
            }
            else
            {
                result = ParentFile.ReadData(Entry, m_lPosition, buffer, length);
            }

            m_lPosition += result;
            return result;
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
        public override void Write(byte[] buffer, int offset, int length)
        {
            if (Stream == null)
            {
                ParentFile.WriteData(Entry, m_lPosition, buffer, offset, length);
            }
            else
            {
                base.Write(buffer, offset, length);

                if (Stream.Length > 32768)
                {
                    ParentFile.SetEntryStream(Entry, Stream);
                    Stream = null;
                }
            }

            m_lPosition += length;
        }
        /// <summary>
        /// Sets the position within the current stream. 
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference
        /// point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (Stream != null)
                Stream.Seek(offset, origin);

            switch (origin)
            {
                case SeekOrigin.Begin:
                    m_lPosition = offset;
                    break;

                case SeekOrigin.Current:
                    m_lPosition += offset;
                    break;

                case SeekOrigin.End:
                    m_lPosition = Entry.Size + offset;
                    break;
            }

            return m_lPosition;
        }
        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            if (Stream != null)
                base.SetLength(value);

            Entry.Size = (uint)value;
        }
        /// <summary>
        /// Gets the length in bytes of the stream. Read-only.
        /// </summary>
        public override long Length
        {
            get
            {
                return (Stream != null) ? Stream.Length : Entry.Size;
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
                m_lPosition = value;

                if (Stream != null)
                    Stream.Position = value;
            }
        }
        /// <summary>
        /// Causes any buffered data to be written to the underlying compound file.
        /// </summary>
        public override void Flush()
        {
            // Here we have to go to parent element and ask him to write this data inside.
            if (Stream != null)
                base.Flush();
        }
        #endregion
    }
}
