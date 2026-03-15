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

using System;

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
	/// <summary>
	/// Summary description for ILengthSetter.
	/// </summary>
	public interface ILengthSetter
	{
    /// <summary>
    /// Sets length of the internal data.
    /// </summary>
    /// <param name="iLength">New length to set.</param>
    void SetLength( int iLength );
	}
}
