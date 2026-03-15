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
namespace Syncfusion.Compression.Zip
{
	/// <summary>
	/// Preprocesses file name before ZipArchiveItem saving. Used to convert full item path into local one.
	/// </summary>
	public interface IFileNamePreprocessor
	{
		/// <summary>
		/// Somehow converts full path into name that will be stored in the zip archive.
		/// </summary>
		/// <param name="fullName">Name to process.</param>
		/// <returns>Converted name.</returns>
		string PreprocessName( string fullName );
	}
}
