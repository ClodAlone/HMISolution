#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Defines data compression level.
    /// </summary>
    public enum PdfCompressionLevel
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Pack without compression.
        /// </summary>
        None = 0,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Use high speed compression, reduce of data size is low.
        /// </summary>
        BestSpeed = 1,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Something middle between normal and BestSpeed compressions.
        /// </summary>
        BelowNormal = 3,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Use normal compression, middle between speed and size.
        /// </summary>
        Normal = 5,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Pack better but require a little more time.
        /// </summary>
        AboveNormal = 7,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Use best compression, slow enough.
        /// </summary>
        Best = 9
    }

    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Enumerator that implements compression level.
    /// </summary>
    internal enum CompressionType
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// No compression.
        /// </summary>
        None,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compresses data using the zlib or deflate compression method,
        /// reproducing the original text or binary data.
        /// </summary>
        Zlib,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compresses data using the LZW compression method, reproducing
        /// the original text or binary data.
        /// </summary>
        LZW,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Compresses data using the ASCII85 compression method, reproducing
        /// the original text or binary data.
        /// </summary>
        ASCII85
    }

    /// <property name="flag" value="Finished" />
    /// <summary>
    /// Represents supported stream filters.
    /// </summary>
    internal enum StreamFilters
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Decompresses data encoded using a DCT (discrete cosine transform)
        /// technique based on the JPEG standard, reproducing image sample
        /// data that approximates the original data.
        /// </summary>
        DCTDecode,

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Decompresses data encoded using the zlib / deflate
        /// compression method, reproducing the original text or binary
        /// data.
        /// </summary>
        FlateDecode,
    }
}
