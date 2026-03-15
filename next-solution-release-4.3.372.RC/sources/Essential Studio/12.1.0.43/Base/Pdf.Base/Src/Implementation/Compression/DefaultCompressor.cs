#region Copyright Syncfusion Inc. 2001 - 2014

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion


using System;
using System.IO;
using Syncfusion.Pdf.Primitives;

namespace Syncfusion.Pdf.Compression
{
    /// <summary>
    /// Class for default compressor. Default compressor does not
    /// compress data.
    /// </summary>
    /// <property name="flag" value="Finished"/>
    internal class DefaultCompressor : IPdfCompressor
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCompressor"/> class.
        /// </summary>
        public DefaultCompressor()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the compressor in PDF format.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public string Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the type of the compressor.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public CompressionType Type
        {
            get
            {
                return CompressionType.None;
            }
        }
        #endregion

        #region IPdfCompressor methods
        /// <summary>
        /// Compresses bytes data.
        /// </summary>
        /// <param name="data">Bytes data that should be compressed.</param>
        /// <returns>Compressed bytes data.</returns>
        /// <property name="flag" value="Finished"/>
        public byte[] Compress(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return data;
        }

        /// <summary>
        /// Compresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data that should be
        /// compressed.</param>
        /// <returns>Compressed streams data.</returns>
        /// <property name="flag" value="Finished"/>
        public Stream Compress(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            return inputStream;
        }

        /// <summary>
        /// Compresses string data.
        /// </summary>
        /// <param name="data">String data that should be compressed.</param>
        /// <returns>Compressed string data.</returns>
        /// <property name="flag" value="Finished"/>
        public byte[] Compress(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return PdfString.StringToByte(data);
        }

        /// <summary>
        /// Decompresses string data.
        /// </summary>
        /// <param name="value">String data that should be decompressed.</param>
        /// <returns>Decompressed bytes data.</returns>
        /// <property name="flag" value="Finished"/>
        public byte[] Decompress(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return PdfString.StringToByte(value);
        }

        /// <summary>
        /// Decompresses bytes data.
        /// </summary>
        /// <param name="value">Bytes data that should be decompressed.</param>
        /// <returns>Decompressed bytes data.</returns>
        /// <property name="flag" value="Finished"/>
        public byte[] Decompress(byte[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return value;
        }

        /// <summary>
        /// Decompresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data that should be
        /// decompressed.</param>
        /// <returns>Decompressed stream data.</returns>
        /// <property name="flag" value="Finished"/>
        public Stream Decompress(Stream inputStream)
        {
            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            return inputStream;
        }
        #endregion
    }
}
