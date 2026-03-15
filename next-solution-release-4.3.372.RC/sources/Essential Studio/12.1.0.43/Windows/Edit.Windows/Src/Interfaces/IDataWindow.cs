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

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Interface of data window - some part of data.
	/// </summary>
	public interface IDataWindow
	{
		/// <summary>
		/// Source for reading data of this stream. More than one windows can use this source.
		/// </summary>
		ISource Source{ get; }
		/// <summary>
		/// Index of first byte of window's data in Source.
		/// </summary>
		long    Start{ get; }
		/// <summary>
		/// Size of window's data.
		/// </summary>
		long    Size{ get; }
		/// <summary>
		/// Position of window in output stream.
		/// </summary>
		long    Position{ get; }
	}
}
