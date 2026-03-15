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
#endregion

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Source of data for data window
	/// </summary>
	public interface ISource
	{
		/// <summary>
		/// Length of data in source.
		/// </summary>
		long Length{ get; }
		/// <summary>
		/// Reads data and writes it to array.
		/// </summary>
		/// <param name="position">Position of data in the source.</param>
		/// <param name="data">Array, where data will be written.</param>
		/// <param name="offset">Offset in array.</param>
		/// <param name="count">Size of data that have to be read.</param>
		/// <returns>Count of actually read bytes.</returns>
		int  GetData( long position, byte[] data, int offset, int count );
	}
}
