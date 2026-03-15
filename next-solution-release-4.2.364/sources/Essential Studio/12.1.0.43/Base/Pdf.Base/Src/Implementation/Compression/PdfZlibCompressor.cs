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

using System;
using System.IO;
using System.Text;
using Syncfusion.Compression;
using Syncfusion.Pdf.Primitives;
#if !SILVERLIGHT && ! WP
using System.IO.Compression;
#endif
namespace Syncfusion.Pdf.Compression
{
    /// <summary>
    /// Compresses data using the zlib / deflate compression
    /// method, reproducing the original text or binary data.
    /// </summary>
    internal class PdfZlibCompressor
        : IPdfCompressor
    {
        #region Constants
        /// <summary>
        /// Default buffer size for decompression.
        /// </summary>
        private const int DefaultBufferSize = 32;
        #endregion

        #region Static fields
        /// <summary>
        /// Name of the compressor.
        /// </summary>
        private static string DefaultName = StreamFilters.FlateDecode.ToString();
        #endregion

        #region Fields
        /// <summary>
        /// Level of compression.
        /// </summary>
        private PdfCompressionLevel m_level;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfZlibCompressor"/> class.
        /// </summary>
        public PdfZlibCompressor()
        {
            m_level = PdfCompressionLevel.Normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfZlibCompressor"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        public PdfZlibCompressor(PdfCompressionLevel level)
            : this()
        {
            m_level = level;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets name of compressor in PDF format.
        /// </summary>
        public string Name
        {
            get
            {
                return DefaultName;
            }
        }

        /// <summary>
        /// Gets type of compressor.
        /// </summary>
        public CompressionType Type
        {
            get
            {
                return CompressionType.Zlib;
            }
        }

        /// <summary>
        /// Gets encoding value for this compressor.
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }

        /// <summary>
        /// Gets or sets level of compression.
        /// </summary>
        public PdfCompressionLevel Level
        {
            get
            {
                return m_level;
            }

            set
            {
                if (m_level != value)
                {
                    m_level = value;
                }
            }
        }
        #endregion

        #region IPDFCompressor methods
        /// <summary>
        /// Compresses bytes data.
        /// </summary>
        /// <param name="data">Bytes data to be compressed.</param>
        /// <returns>Compressed bytes data.</returns>
        public byte[] Compress(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            byte[] output = null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (Stream outputStream = Compress(stream))
                {
                    output = PdfStream.StreamToBytes(outputStream);
                }
            }

            return output;
        }

        /// <summary>
        /// Compresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data.</param>
        /// <returns>Compressed streams data.</returns>
        /// <property name="flag" value="Finished"/>
        public Stream Compress(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            // NOTE: The caller must dispose this stream.
            MemoryStream outputStream = new MemoryStream();
            PdfCompressionLevel level = (PdfCompressionLevel)Level;

            Syncfusion.Compression.CompressionLevel cLevel = (Syncfusion.Compression.CompressionLevel)level;

            CompressedStreamWriter output = new CompressedStreamWriter(outputStream, cLevel, false);
            byte[] buffer = new byte[inputStream.Length];
            inputStream.Position = 0;
            inputStream.Read(buffer, 0, buffer.Length);

            output.Write(buffer, 0, buffer.Length, true);
            return outputStream;
        }

        /// <summary>
        /// Compresses string data.
        /// </summary>
        /// <param name="data">String data to be compressed.</param>
        /// <returns>Compressed string data.</returns>
        public byte[] Compress(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            byte[] output = null;
            byte[] inputBytes = Encoding.GetBytes(data);

            using (MemoryStream stream = new MemoryStream(inputBytes))
            {
                using (Stream outputStream = Compress(stream))
                {
                    output = PdfStream.StreamToBytes(outputStream);
                }
            }

            return output;
        }

        /// <summary>
        /// Decompresses string data.
        /// </summary>
        /// <param name="data">String data to be decompressed.</param>
        /// <returns>Decompressed bytes data.</returns>
        public byte[] Decompress(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            byte[] output = null;
            byte[] inputBytes = Encoding.GetBytes(data);

            using (MemoryStream stream = new MemoryStream(inputBytes))
            {
                using (Stream outputStream = Decompress(stream))
                {
                    output = PdfStream.StreamToBytes(outputStream);
                }
            }

            return output;
        }

        /// <summary>
        /// Decompresses bytes data.
        /// </summary>
        /// <param name="data">Bytes data to be decompressed.</param>
        /// <returns>Decompressed bytes data.</returns>
        public byte[] Decompress(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Length == 0) return data;

            byte[] output = null;

            using (MemoryStream stream = new MemoryStream(data))
            {
                using (Stream outputStream = Decompress(stream))
                {
                    output = PdfStream.StreamToBytes(outputStream);
                }
            }

            return output;
        }

        /// <summary>
        /// Decompresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data to be decompressed.</param>
        /// <returns>Decompressed stream data.</returns>
        public Stream Decompress(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            // NOTE: The caller must dispose this stream.
            MemoryStream outputStream = new MemoryStream();
            byte[] buffer = new byte[DefaultBufferSize];

            CompressedStreamReader extractor = new CompressedStreamReader(inputStream);
            int len;

            // Workaround for wrong block length.
            try
            {
                while ((len = extractor.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, len);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Wrong block length.")
                {
                    inputStream.Position = 0;
                    extractor = new CompressedStreamReader(inputStream);
                    buffer = new byte[1];
                    outputStream = new MemoryStream();
                    try
                    {
                        while ((len = extractor.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, len);
                        }
                    }
                    catch
                    { }
                }
                else if (ex.Message == "Checksum check failed.")
                {
#if !SILVERLIGHT && !WP
                    try
                    {
                        inputStream.Position = 0;
                        inputStream.ReadByte();
                        inputStream.ReadByte();
                        using (DeflateStream s = new DeflateStream(inputStream, CompressionMode.Decompress, true))
                        {
                            buffer = new byte[4096];
                            outputStream = new MemoryStream();
                            do
                            {
                                int bytesread = s.Read(buffer, 0, 4096);
                                if (bytesread <= 0)
                                    break;
                                outputStream.Write(buffer, 0, bytesread);
                            } while (true);
                        }
                    }
                    catch
                    { }
#endif
                }
                else
                    throw;
            }
            return outputStream;
        }
        #endregion
    }
}
