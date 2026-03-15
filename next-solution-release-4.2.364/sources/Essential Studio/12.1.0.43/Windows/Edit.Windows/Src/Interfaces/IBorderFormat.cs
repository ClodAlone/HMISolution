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
using System.Drawing;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Text border settings.
	/// </summary>
	public interface IBorderFormat
	{
		/// <summary>
		/// Border color.
		/// </summary>
		Color BorderColor{ get; set; }
		/// <summary>
		/// Border line style. No border if 'None'.
		/// </summary>
		FrameBorderStyle BorderStyle{ get; set; }
		/// <summary>
		/// Border line weight.
		/// </summary>
		BorderWeight BorderWeight{ get; set; }
	}
}
