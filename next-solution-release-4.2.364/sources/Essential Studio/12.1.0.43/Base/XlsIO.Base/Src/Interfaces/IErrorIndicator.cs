#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
#endregion

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Represents IErrorIndicator interface.
	/// </summary>
	public interface IErrorIndicator
	{
    #region Class properties
    /// <summary>
    /// Represents error indicator ignore options.
    /// </summary>
    ExcelIgnoreError IgnoreOptions { get; set; }
    #endregion
	}
}
