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
using System.Collections;

namespace Syncfusion.Windows.Forms.Edit.Interfaces
{
	/// <summary>
	/// Result of FindNext method.
	/// </summary>
	public enum FindNextResult
	{
		/// <summary>
		/// Text was found.
		/// </summary>
		Ok,
		/// <summary>
		/// Text wasn't found.
		/// </summary>
		NotFound,
		/// <summary>
		/// Error occured.
		/// </summary>
		Error
	}

	/// <summary>
	/// Interface for Find dialog form.
	/// </summary>
	public interface IFindDialogForm
	{
		#region Interface Properties
		/// <summary>
		/// Gets or sets searching text.
		/// </summary>
		string SearchText{ get; set; }
		/// <summary>
		/// Gets find history.
		/// </summary>
		ArrayList History{ get; }
		#endregion

		#region Interface Methods
		/// <summary>
		/// Selects text in find combo box and focuses it.
		/// </summary>
		void SelectTextAndFocus();
		/// <summary>
		/// Invokes searching process.
		/// </summary>
		FindNextResult FindNext();
		/// <summary>
		/// Shows the form.
		/// </summary>
		void Show();
		#endregion
	}
	/// <summary>
	/// Interface for Replace dialog form.
	/// </summary>
	public interface IReplaceDialogForm
		: IFindDialogForm
	{
		#region Interface Properties
		/// <summary>
		/// Gets replace history.
		/// </summary>
		ArrayList ReplaceHistory{ get; }
		#endregion
	}
}