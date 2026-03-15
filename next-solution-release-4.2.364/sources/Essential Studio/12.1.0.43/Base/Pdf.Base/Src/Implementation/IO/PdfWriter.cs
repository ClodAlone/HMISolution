#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Text;

using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.IO
{
#if NETFX_CORE || WP
    public class PdfWriter : IPdfWriter, IDisposable
#else
    internal class PdfWriter:IPdfWriter, IDisposable
#endif
    {
        #region Fields
        private Stream m_stream;
        //private Stream m_intermediateStream;
        private PdfDocumentBase m_document;
        private bool m_cannotSeek;
        private long m_position;
        private long m_length;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the stream.
        /// </summary>
        private Stream Stream
        {
            get
            {
                //Stream stream = ( m_intermediateStream == null ) ? m_stream : m_intermediateStream;
                Stream stream = GetStream();
                return stream;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal PdfWriter(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (!stream.CanWrite)
                throw new ArgumentException("Can't write to the specified stream", "stream");

            m_stream = stream;

            if (!stream.CanRead || !stream.CanSeek)
            {
                //m_intermediateStream = new MemoryStream( 1024 );
                m_cannotSeek = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            //if( m_intermediateStream != null )
            //{
            //  ( m_intermediateStream as MemoryStream ).WriteTo( m_stream );
            //  m_intermediateStream.Close();
            //}

            if (m_stream != null)
            {
                m_stream.Flush();
                m_stream = null;
            }
        }
        #endregion

        #region IPdfWriter Members

        /// <summary>
        /// Gets or Sets the document required for saving process.
        /// </summary>
        public PdfDocumentBase Document
        {
            get
            {
                return m_document;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Document");

                m_document = value;
            }
        }

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        public long Position
        {
            get
            {
                if (m_cannotSeek)
                {
                    return m_position;
                }

                //if( m_intermediateStream != null )
                //{
                //  return m_intermediateStream.Position;
                //}
                else
                {
                    return m_stream.Position;
                }
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(
                        "Position", "The stream position can't be less then zero.");

                //if( m_intermediateStream != null )
                //{
                //  m_intermediateStream.Position = value;
                //}
                //else
                {
                    m_stream.Position = value;
                }
            }
        }

        /// <summary>
        /// Gets stream length.
        /// </summary>
        public long Length
        {
            get
            {
                if (m_cannotSeek)
                {
                    return m_length;
                }

                //if( m_intermediateStream != null )
                //{
                //  return m_intermediateStream.Length;
                //}
                else
                {
                    return m_stream.Length;
                }
            }
        }

        /// <summary>
        /// Writes the specified PDF object.
        /// </summary>
        /// <param name="pdfObject">The PDF object.</param>
        public void Write(IPdfPrimitive pdfObject)
        {
            pdfObject.Save(this);
        }

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        public void Write(long number)
        {
            PdfNumber num = new PdfNumber(number);

            num.Save(this);
        }

        /// <summary>
        /// Writes the specified number.
        /// </summary>
        /// <param name="number">The number.</param>
        public void Write(float number)
        {
            PdfNumber num = new PdfNumber(number);

            num.Save(this);
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Write(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Write(data);
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Write(char[] text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Write(byte[] data)
        {
            Stream stream = GetStream();
            int length = data.Length;

            m_length += length;
            m_position += length;

            stream.Write(data, 0, length);
        }

        #endregion

        #region Helper methods
        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <returns>The stream.</returns>
        internal Stream GetStream()
        {
            Stream stream;

            //if( m_intermediateStream != null )
            //{
            //  stream = m_intermediateStream;
            //}
            //else
            {
                stream = m_stream;
            }

            return stream;
        }
        #endregion
    }
}
