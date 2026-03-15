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

#region file using directives
using System;
#endregion

namespace Syncfusion.Compression.Zip
{
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

	/// <summary>
	/// General purpose bit flag.
	/// </summary>
	[ Flags ]
	public enum GeneralPurposeBitFlags : short
	{
		/// <summary>
		/// If this bit is set, the fields crc-32, compressed size and uncompressed
		/// size are set to zero in the local header.  The correct values are put
		/// in the data descriptor immediately following the compressed data.
		/// (Note: PKZIP version 2.04g for DOS only recognizes this bit for method 8
		/// compression, newer versions of PKZIP recognize this bit for any compression method.)
		/// </summary>
		SizeAfterData = 0x0008,
		/// <summary>
		/// Language encoding flag (EFS).  If this bit is set, the filename and
		/// comment fields for this file must be encoded using UTF-8.
		/// </summary>
		Unicode = 0x0800,
	}
}
