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
    /// Compresses data using the LZW compression
    /// method, reproducing the original text or binary data.
    /// </summary>
    internal class PdfLzwCompressor : IPdfCompressor
    {
        #region Constants
        /// <summary>
        /// EndOfData marker.
        /// </summary>
        private const int c_eod = 257;

        /// <summary>
        /// Clear-table marker.
        /// </summary>
        private const int c_clearTable = 256;

        /// <summary>
        /// Start code marker.
        /// </summary>
        private const int c_startCode = 258;

        /// <summary>
        /// The marker to set the dictionary to 10 bits code length.
        /// </summary>
        private const int c_10BitsCode = 511;

        /// <summary>
        /// The marker to set the dictionary to 11 bits code length
        /// </summary>
        private const int c_11BitsCode = 1023;

        /// <summary>
        /// The marker to set the dictionary to 12 bits code length
        /// </summary>
        private const int c_12BitsCode = 2047;

        #endregion

        #region Fields
        /// <summary>
        /// Table for codes (dictionary).
        /// </summary>
        private byte[][] m_codeTable;

        /// <summary>
        /// Input data.
        /// </summary>
        private byte[] m_inputData;

        /// <summary>
        /// Output data.
        /// </summary>
        private Stream m_outputData;

        /// <summary>
        /// Table index.
        /// </summary>
        private int m_tableIndex;

        /// <summary>
        /// The number of bits per code.
        /// </summary>
        private int m_bitsToGet;

        /// <summary>
        /// Byte read.
        /// </summary>
        private int m_byteRead;

        /// <summary>
        /// Next data.
        /// </summary>
        private int m_nextData;

        /// <summary>
        /// Nex bits.
        /// </summary>
        private int m_nextBits;

        /// <summary>
        /// The size of the table.
        /// </summary>
        private int[] m_sizeTable;
        #endregion

        #region Class initialize/finalize
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLzwCompressor"/> class.
        /// </summary>
        public PdfLzwCompressor()
        {
            m_sizeTable = new int[] { 511, 1023, 2047, 4095 };
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
                return CompressionType.LZW;
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
            if (outputData == null)
            {
                throw new ArgumentNullException("outputData");
            }

            if (inputData == null)
            {
                throw new ArgumentNullException("inputData");
            }

            m_inputData = inputData;
            m_outputData = outputData;

            m_byteRead = 0;

            m_nextData = 0;
            m_nextBits = 0;
            m_bitsToGet = 9;

            int code = 0;
            int oldCode = 0;
            byte[] data;

            while ((code = NewCode()) != c_eod)
            {
                if (code == c_clearTable)
                {
                    InitializeDataTable();
                    code = NewCode();

                    if (code == c_eod)
                    {
                        break;
                    }

                    WriteCode(m_codeTable[code]);
                    oldCode = code;
                }
                else
                {
                    if (code < m_tableIndex)
                    {
                        data = m_codeTable[code];

                        WriteCode(data);
                        AddCodeToTable(m_codeTable[oldCode], data[0]);
                        oldCode = code;
                    }
                    else
                    {
                        data = m_codeTable[oldCode];
                        data = UniteBytes(data, data[0]);
                        WriteCode(data);
                        AddCodeToTable(data);
                        oldCode = code;
                    }
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
                // TODO:  Add PDFLZWCompressor.Name getter implementation
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
            // TODO:  Add PDFLZWCompressor.Compress implementation
            return null;
        }

        /// <summary>
        /// Compresses string data.
        /// </summary>
        /// <param name="data">String data to be compressed.</param>
        /// <returns>Compressed string data.</returns>
        public byte[] Compress(string data)
        {
            // TODO:  Add PDFLZWCompressor.Syncfusion.Pdf.IPDFCompressor.Compress implementation
            return null;
        }

        /// <summary>
        /// Compresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data to be compressed.</param>
        /// <returns>Compressed streams data.</returns>
        public Stream Compress(Stream inputStream)
        {
            // TODO:  Add PDFLZWCompressor.Syncfusion.Pdf.IPDFCompressor.Compress implementation
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

            if (stream.Length > 0)
            {
                stream.Read(data, 0, (int)stream.Length - 1);
            }

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
        /// Initializes the data table.
        /// </summary>
        private void InitializeDataTable()
        {
            m_codeTable = new byte[8192][];

            for (int i = 0; i < 256; i++)
            {
                m_codeTable[i] = new byte[1];
                m_codeTable[i][0] = (byte)i;
            }

            m_tableIndex = c_startCode;
            m_bitsToGet = 9;
        }

        /// <summary>
        /// Writes the code to output stream.
        /// </summary>
        /// <param name="code">The code.</param>
        private void WriteCode(byte[] code)
        {
            m_outputData.Write(code, 0, code.Length);
        }

        /// <summary>
        /// Adds the code to table.
        /// </summary>
        /// <param name="oldBytes">The old bytes.</param>
        /// <param name="newByte">The new byte.</param>
        private void AddCodeToTable(byte[] oldBytes, byte newByte)
        {
            int length = oldBytes.Length;
            byte[] data = new byte[length + 1];
            Array.Copy(oldBytes, 0, data, 0, length);

            data[length] = newByte;

            AddCodeToTable(data);
        }

        /// <summary>
        /// Adds the code to table.
        /// </summary>
        /// <param name="data">The data.</param>
        private void AddCodeToTable(byte[] data)
        {
            m_codeTable[m_tableIndex++] = data;

            if (m_tableIndex == c_10BitsCode)
            {
                m_bitsToGet = 10;
            }
            else if (m_tableIndex == c_11BitsCode)
            {
                m_bitsToGet = 11;
            }
            else if (m_tableIndex == c_12BitsCode)
            {
                m_bitsToGet = 12;
            }
        }

        /// <summary>
        /// Add new data to the olddata array.
        /// </summary>
        /// <param name="oldData">The old data.</param>
        /// <param name="newData">The new data.</param>
        /// <returns>Result data array.</returns>
        private byte[] UniteBytes(byte[] oldData, byte newData)
        {
            int length = oldData.Length;
            byte[] data = new byte[length + 1];
            Array.Copy(oldData, 0, data, 0, length);
            data[length] = newData;

            return data;
        }

        /// <summary>
        /// News the code.
        /// </summary>
        /// <returns>code</returns>
        private int NewCode()
        {
            m_nextData = (m_nextData << 8) | m_inputData[m_byteRead++];
            m_nextBits += 8;

            if (m_nextBits < m_bitsToGet)
            {
                m_nextData = (m_nextData << 8) | m_inputData[m_byteRead++];
                m_nextBits += 8;
            }

            int code = (m_nextData >> (m_nextBits - m_bitsToGet)) & m_sizeTable[m_bitsToGet - 9];
            m_nextBits -= m_bitsToGet;

            return code;
        }
        #endregion
    }
}
