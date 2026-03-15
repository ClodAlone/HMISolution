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

using Syncfusion.XlsIO.Implementation;
#endregion

namespace Syncfusion.XlsIO.Interfaces
{
	/// <summary>
	/// Summary description for IInternalFont.
	/// </summary>
	public interface IInternalFont : IFont
	{
    /// <summary>
    /// Returns font index. Read-only.
    /// </summary>
    int Index { get; }
    /// <summary>
    /// Returns FontImpl for current font. Read-only.
    /// </summary>
    FontImpl Font { get; }
	}
}
