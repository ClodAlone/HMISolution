#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Engine for parsing and creation of header/footer strings.
	/// </summary>
	public interface IHFEngine : IRichTextString
	{
    /// <summary>
    /// Parses text of header/footer part.
    /// </summary>
    /// <param name="strText">Text of header/footer part.</param>
    void Parse( string strText );
    /// <summary>
    /// Returns string in format that is supported by Excel header/footer.
    /// </summary>
    /// <returns>String in format that is supported by Excel header/footer.</returns>
    string GetHeaderFooterString();
  }
}
