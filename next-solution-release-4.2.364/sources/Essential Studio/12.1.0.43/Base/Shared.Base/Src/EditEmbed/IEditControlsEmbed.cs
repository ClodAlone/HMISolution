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

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Implement this interface in a class if the class embeds one or more
	/// edit controls (text boxes, combo boxes). Implementing this interface
	/// provides a way for external classes to access the embedded edit
	/// control(s).
	/// </summary>
	public interface IEditControlsEmbed
	{
		/// <summary>
		/// Returns the active edit control (text box, combo box).
		/// </summary>
		/// <param name="listener">The IEditControlsEmbedListener based auto complete control.</param>
		/// <remarks>
		/// The implementation of this method will let the AutoComplete
		/// control provide a link back to it so that it can be informed
		/// of any changes in the active edit control.
		/// <para>
		/// This is used when there are more than one edit controls on one 
		/// composite control. In this case, the AutoComplete control
		/// will be informed when there is a change in focus between
		/// the different edit controls. This is assuming that the AutoComplete
		/// control is not able to sense the change in focus.
		/// </para>
		/// </remarks>
		Control GetActiveEditControl(IEditControlsEmbedListener listener);
	}

	public interface IEditControlsEmbedListener
	{
		void SetActiveEditControl(IEditControlsEmbed parentControl, Control editControl);
	}
}