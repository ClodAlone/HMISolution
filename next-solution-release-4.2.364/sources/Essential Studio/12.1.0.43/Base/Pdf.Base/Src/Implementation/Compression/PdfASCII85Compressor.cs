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

namespace Syncfusion.Pdf.Compression
{
    /// <summary>
    /// Compresses data using the ASCII85 compression
    /// method, reproducing the original text or binary data.
    /// </summary>
    internal class PdfASCII85Compressor : IPdfCompressor
    {
        #region Fields
        /// <summary>
        /// Internal variable contains the ascii offset.
        /// </summary>
        private const int m_asciiOffset = 33;

        /// <summary>
        /// Internal variable contains the encoded block.
        /// </summary>
        private byte[] m_encodedBlock = new byte[5];

        /// <summary>
        /// Internal variable contains the decoded block.
        /// </summary>
        private byte[] m_decodedBlock = new byte[4];

        /// <summary>
        /// Internal variable.
        /// </summary>
        private uint m_tuple = 0;

        /// <summary>
        /// The code Table.
        /// </summary>
        private uint[] m_codeTable;
        #endregion

        #region Class initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfASCII85Compressor"/> class.
        /// </summary>
        public PdfASCII85Compressor()
        {
            m_codeTable = new uint[] { 85 * 85 * 85 * 85, 85 * 85 * 85, 85 * 85, 85, 1 };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Type of the compressor.
        /// </summary>
        /// <value></value>
        public CompressionType Type
        {
            get
            {
                return CompressionType.ASCII85;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Decompresses the specified input data.
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <param name="outputData">The output data.</param>
        public void Decompress(byte[] inputData, Stream outputData)
        {
            int count = 0;
            bool processChar = false;

            foreach (byte b in inputData)
            {
                char c = Convert.ToChar(b);
                switch (c)
                {
                    case 'z':
                        if (count != 0)
                        {
                            throw new PdfException("The character 'z' is invalid inside an ASCII85 block.");
                        }

                        m_decodedBlock[0] = 0;
                        m_decodedBlock[1] = 0;
                        m_decodedBlock[2] = 0;
                        m_decodedBlock[3] = 0;
                        outputData.Write(m_decodedBlock, 0, m_decodedBlock.Length);
                        processChar = false;
                        break;

                    case '\n':
                    case '\r':
                    case '\t':
                    case '\0':
                    case '\f':
                    case '\b':
                        processChar = false;
                        break;

                    default:
                        processChar = true;
                        break;
                }

                if (processChar)
                {
                    m_tuple += ((uint)(c - m_asciiOffset) * m_codeTable[count]);
                    count++;
                    if (count == m_encodedBlock.Length)
                    {
                        DecodeBlock();
                        outputData.Write(m_decodedBlock, 0, m_decodedBlock.Length);
                        m_tuple = 0;
                        count = 0;
                    }
                }
            }

            if (count != 0)
            {
                count--;
                m_tuple += m_codeTable[count];
                DecodeBlock(count);
                for (int i = 0; i < count; i++)
                {
                    outputData.WriteByte(m_decodedBlock[i]);
                }
            }
        }
        #endregion

        #region IPDFCompressor Members
        /// <summary>
        /// Gets name of compressor in PDF format.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get
            {
                // TODO:  Add PdfASCII85Compressor.Name getter implementation
                return null;
            }
        }

        /// <summary>
        /// Compresses bytes data.
        /// </summary>
        /// <param name="data">Bytes data to be compressed.</param>
        /// <returns>Compressed bytes data.</returns>
        public byte[] Compress(byte[] data)
        {
            // TODO:  Add PdfASCII85Compressor.Compress implementation
            return null;
        }

        /// <summary>
        /// Compresses string data.
        /// </summary>
        /// <param name="data">String data to be compressed.</param>
        /// <returns>Compressed string data.</returns>
        public byte[] Compress(string data)
        {
            // TODO:  Add PdfASCII85Compressor.Syncfusion.Pdf.IPDFCompressor.Compress implementation
            return null;
        }

        /// <summary>
        /// Compresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data to be compressed.</param>
        /// <returns>Compressed streams data.</returns>
        public Stream Compress(Stream inputStream)
        {
            // TODO:  Add PdfASCII85Compressor.Syncfusion.Pdf.IPDFCompressor.Compress implementation
            return null;
        }

        /// <summary>
        /// Decompresses string data.
        /// </summary>
        /// <param name="value">String data to be decompressed.</param>
        /// <returns>Decompressed bytes data.</returns>
        public byte[] Decompress(string value)
        {
            byte[] data;
            byte[] inputBytes = Encoding.UTF8.GetBytes(value);
            MemoryStream stream = new MemoryStream();

            Decompress(inputBytes, stream);
            data = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(data, 0, (int)stream.Length - 1);

            return data;
        }

        /// <summary>
        /// Decompresses bytes data.
        /// </summary>
        /// <param name="value">Bytes data to be decompressed.</param>
        /// <returns>decompressed bytes data</returns>
        public byte[] Decompress(byte[] value)
        {
            MemoryStream stream = new MemoryStream();

            Decompress(value, stream);
            byte[] data = new byte[stream.Length];

            stream.Position = 0;
            stream.Read(data, 0, (int)stream.Length - 1);

            return data;
        }

        /// <summary>
        /// Decompresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data to be decompressed.</param>
        /// <returns>Decompressed stream data.</returns>
        public Stream Decompress(Stream inputStream)
        {
            MemoryStream stream = new MemoryStream();
            byte[] value = new byte[inputStream.Length];
            inputStream.Position = 0;
            inputStream.Read(value, 0, (int)inputStream.Length - 1);
            Decompress(value, stream);
            return stream;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Decodes the block.
        /// </summary>
        private void DecodeBlock()
        {
            DecodeBlock(m_decodedBlock.Length);
        }

        /// <summary>
        /// Decodes the block.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        private void DecodeBlock(int bytes)
        {
            for (int i = 0; i < bytes; i++)
            {
                m_decodedBlock[i] = (byte)(m_tuple >> 24 - (i * 8));
            }
        }
        #endregion
    }
}
