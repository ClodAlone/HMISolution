#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Interfaces
{
	/// <summary>
	/// Classes that implement this interface are responsible for whole
	/// workbook serialization into stream or into file.
	/// </summary>
	public interface IWorkbookSerializator
	{
#if !(WINRT )
    /// <summary>
    /// Saves workbook into specified file.
    /// </summary>
    /// <param name="fullName">Destination file name.</param>
    /// <param name="book">Workbook to save.</param>
    /// <param name="saveType">Save type.</param>
    void Serialize( string fullName, WorkbookImpl book, ExcelSaveType saveType );
#endif
    /// <summary>
    /// Saves workbook into stream.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="book">Workbook to save.</param>
    /// <param name="saveType">Save type (template or ordinary xls).</param>
    void Serialize( Stream stream, WorkbookImpl book, ExcelSaveType saveType );
  }
}
