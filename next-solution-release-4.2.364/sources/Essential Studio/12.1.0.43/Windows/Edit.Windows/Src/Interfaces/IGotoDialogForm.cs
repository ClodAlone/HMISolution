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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Interface for Goto dialog form..
	/// </summary>
	public interface IGotoDialogForm
	{
		#region Interface Properties
		/// <summary>
		/// Gets or sets minimum line number.
		/// </summary>
		int MinLine{ get; set; }
		/// <summary>
		/// Gets or sets maximum line number.
		/// </summary>
		int MaxLine{ get; set; }
		/// <summary>
		/// Gets line number.
		/// </summary>
		int LineNumber{ get; }
		#endregion

		#region Interface Methods
		/// <summary>
		/// Shows the form in dialog mode.
		/// </summary>
		DialogResult ShowDialog();
		#endregion
	}
}
