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

using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

namespace Syncfusion.XlsIO.Interfaces
{
	/// <summary>
	/// Interface that contains method for to take native ptg.
	/// </summary>
	public interface INativePTG
	{
    /// <summary>
    /// Gets ptg of current range.
    /// </summary>
    /// <returns>Returns native ptg.</returns>
    Ptg[] GetNativePtg();
	}
}
