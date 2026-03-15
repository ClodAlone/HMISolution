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
    /// Class contains all constants that are required by ZipArchive.
    /// </summary>
  [ CLSCompliant( false ) ]
	public sealed class Constants
	{
		#region Constants
		/// <summary>
		/// Zip header signature.
		/// </summary>
		public const int HeaderSignature = 0x04034b50;
		/// <summary>
		/// Number of bytes in HeaderSignature constant.
		/// </summary>
		public const int HeaderSignatureBytes = 4;
		/// <summary>
		/// Buffer size.
		/// </summary>
		public const int BufferSize = 4096;
		/// <summary>
		/// Version needed to extract.
		/// </summary>
		public const short VersionNeededToExtract = 20;
		/// <summary>
		/// Version made by.
		/// </summary>
		public const short VersionMadeBy = 45;
		/// <summary>
		/// Size of the short value in bytes.
		/// </summary>
		public const int ShortSize = 2;
		/// <summary>
		/// Size of the int value in bytes.
		/// </summary>
		public const int IntSize = 4;
		/// <summary>
		/// Central header signature.
		/// </summary>
		public const int CentralHeaderSignature = 0x02014b50;
		/// <summary>
		/// End of central directory signature.
		/// </summary>
		public const int CentralDirectoryEndSignature = 0x06054b50;
		/// <summary>
		/// Initial value for CRC-32 evaluation.
		/// </summary>
		public const uint StartCrc = 0xFFFFFFFF;
        /// <summary>
        /// Offset to the size field in the End of central directory record.
        /// </summary>
        public const int CentralDirSizeOffset = 12;
        /// <summary>
        /// Start byte of the Header signature.
        /// </summary>
        public const int HeaderSignatureStartByteValue = 80;
		#endregion

		#region Constructors
		/// <summary>
		/// Default constructor to prevent users from creating instances of this class.
		/// </summary>
		private Constants()
		{
		}
		#endregion
	}
}
