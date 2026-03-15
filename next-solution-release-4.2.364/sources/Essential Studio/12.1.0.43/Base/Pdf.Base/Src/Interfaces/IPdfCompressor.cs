#region Copyright Syncfusion Inc. 2001 - 2014

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion

#region file using directives
using System;
using System.Diagnostics;
using System.IO;
#endregion

namespace Syncfusion.Pdf.Compression
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Summary description of IPDFCompressor.
    /// </summary>
    internal interface IPdfCompressor
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the compressor type.
        /// </summary>
        CompressionType Type { get; }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the compressor name in PDF format.
        /// </summary>
        string Name { get; }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compresses bytes data.
        /// </summary>
        /// <param name="data">Bytes data.</param>
        /// <returns>
        /// Compressed bytes data.
        /// </returns>

        byte[] Compress(byte[] data);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compresses string data.
        /// </summary>
        /// <param name="data">String data.</param>
        /// <returns>
        /// Compressed string data.
        /// </returns>
        byte[] Compress(string data);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data.</param>
        /// <returns>
        /// Compressed streams data.
        /// </returns>
        Stream Compress(Stream inputStream);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Decompresses string data.
        /// </summary>
        /// <param name="value">String data.</param>
        /// <returns>
        /// Decompressed bytes data.
        /// </returns>
        byte[] Decompress(string value);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Decompresses bytes data.
        /// </summary>
        /// <param name="value">Bytes data.</param>
        /// <returns>
        /// Decompressed bytes data.
        /// </returns>
        byte[] Decompress(byte[] value);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Decompresses stream data.
        /// </summary>
        /// <param name="inputStream">Stream data.</param>
        /// <returns>
        /// Decompressed stream data.
        /// </returns>
        Stream Decompress(Stream inputStream);
    }
}
