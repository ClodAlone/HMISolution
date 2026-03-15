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

namespace Syncfusion.Drawing
{
	/// <summary>
	/// Specifies the BrushStyle used by <see cref="BrushInfo"/>.
	/// </summary>
	public enum BrushStyle 
	{
		/// <summary>
		/// The <see cref="BrushInfo"/> is an empty object.
		/// </summary>
		None = 0,

		/// <summary>
		/// The <see cref="BrushInfo"/> represents a solid fill.
		/// </summary>
		Solid,

		/// <summary>
		/// The <see cref="BrushInfo"/> represents a pattern fill.
		/// </summary>
		Pattern,

		/// <summary>
		/// The <see cref="BrushInfo"/> represents a gradient fill.
		/// </summary>
		Gradient
	}

}
