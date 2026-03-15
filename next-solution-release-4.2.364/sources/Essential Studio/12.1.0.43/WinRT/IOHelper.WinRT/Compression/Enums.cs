#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if XLSIO
namespace Syncfusion.Compression
#else
namespace Syncfusion.Compression.Zip
#endif
{
    /// <summary>
    /// Compression level.
    /// </summary>
    public enum CompressionLevel
    {
        /// <summary>
        /// Pack without compression
        /// </summary>
        NoCompression = 0,
        /// <summary>
        /// Use high speed compression, reduce of data size is low
        /// </summary>
        BestSpeed = 1,
        /// <summary>
        /// Something middle between normal and BestSpeed compressions
        /// </summary>
        BelowNormal = 3,
        /// <summary>
        /// Use normal compression, middle between speed and size
        /// </summary>
        Normal = 5,
        /// <summary>
        /// Pack better but require a little more time
        /// </summary>
        AboveNormal = 7,
        /// <summary>
        /// Use best compression, slow enough
        /// </summary>
        Best = 9
    }
    /// <summary>
    /// The kind of compression used for an entry in an archive
    /// </summary>
    public enum CompressionMethod
    {
        /// <summary>
        /// The file is stored (no compression).
        /// </summary>
        Stored = 0,
        /// <summary>
        /// The file is Shrunk.
        /// </summary>
        Shrunk = 1,
        /// <summary>
        /// The file is Reduced with compression factor 1.
        /// </summary>
        ReducedFactor1 = 2,
        /// <summary>
        /// The file is Reduced with compression factor 2.
        /// </summary>
        ReducedFactor2 = 3,
        /// <summary>
        /// The file is Reduced with compression factor 3.
        /// </summary>
        ReducedFactor3 = 4,
        /// <summary>
        /// The file is Reduced with compression factor 4.
        /// </summary>
        ReducedFactor4 = 5,
        /// <summary>
        /// The file is Imploded.
        /// </summary>
        Imploded = 6,
        /// <summary>
        /// Reserved for Tokenizing compression algorithm.
        /// </summary>
        Tokenizing = 7,
        /// <summary>
        /// The file is Deflated.
        /// </summary>
        Deflated = 8,
        /// <summary>
        /// Enhanced Deflating using Deflate64(tm).
        /// </summary>
        Defalte64 = 9,
        /// <summary>
        /// PKWARE Data Compression Library Imploding (old IBM TERSE).
        /// </summary>
        PRWARE = 10,
        /// <summary>
        /// File is compressed using BZIP2 algorithm.
        /// </summary>
        BZIP2 = 12,
        /// <summary>
        /// LZMA (EFS).
        /// </summary>
        LZMA = 14,
        /// <summary>
        /// File is compressed using IBM TERSE (new).
        /// </summary>
        IBMTerse = 18,
        /// <summary>
        /// IBM LZ77 z Architecture (PFS).
        /// </summary>
        LZ77 = 19,
        /// <summary>
        /// PPMd version I, Rev 1.
        /// </summary>
        PPMd = 98,
    }
}
