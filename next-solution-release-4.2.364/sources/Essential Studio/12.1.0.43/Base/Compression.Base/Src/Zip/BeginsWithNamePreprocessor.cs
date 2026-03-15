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

#endregion

namespace Syncfusion.Compression.Zip
{
	/// <summary>
	/// Implemenation of IFileNamePreprocessor interface that simply removes
	/// some string from the name start and converts all \ characters into /.
	/// </summary>
	public class BeginsWithNamePreprocessor : 
		IFileNamePreprocessor
	{
		#region Members
		/// <summary>
		/// String to remove from the name start.
		/// </summary>
		private string m_strStartToRemove;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes new instance of the name preprocessor.
		/// </summary>
		/// <param name="startToRemove">String to remove from the name start.</param>
		public BeginsWithNamePreprocessor( string startToRemove )
		{
			m_strStartToRemove = startToRemove;
		}
		#endregion

		#region IFileNamePreprocessor Members
		/// <summary>
		/// Somehow converts full path into name that will be stored in the zip archive.
		/// </summary>
		/// <param name="fullName">Name to process.</param>
		/// <returns>Converted name.</returns>
		public string PreprocessName( string fullName )
		{
			string strResult = fullName;

			if( m_strStartToRemove != null && fullName.StartsWith( m_strStartToRemove ) )
			{
				strResult = fullName.Remove( 0, m_strStartToRemove.Length );
			}

      return strResult;
		}
		#endregion
	}
}
