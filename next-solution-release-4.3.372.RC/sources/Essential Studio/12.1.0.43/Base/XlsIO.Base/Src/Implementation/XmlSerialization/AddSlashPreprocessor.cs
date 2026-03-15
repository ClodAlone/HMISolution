#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.Compression.Zip;

namespace Syncfusion.XlsIO.Implementation.XmlSerialization
{
	/// <summary>
	/// Adds slash ('/') before the file name if necessary.
	/// </summary>
	public class AddSlashPreprocessor : IFileNamePreprocessor
	{
    #region Constructors
    /// <summary>
    /// Initializes a new instance of the AddSlashPreprocessor class.
    /// </summary>
    public AddSlashPreprocessor()
    {
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
      if( fullName != null && fullName.Length > 0 && fullName[ 0 ] != '/' )
      {
        fullName = '/' + fullName;
      }

      return fullName;
    }

    #endregion
  }
}
