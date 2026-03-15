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

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// Specifies the border types that can be assigned to the 
	/// <see cref="AutoComplete"/> popup control.
	/// </summary>
	/// <remarks>You can set this value through the designer for the 
	/// <see cref="AutoComplete.BorderType"/> property.</remarks>
	[Serializable]
	public enum AutoCompleteBorderTypes 
	{
		/// <summary>
		/// The popup control's border will be fixed and the user will not
		/// be able to resize it.
		/// </summary>
		Fixed =0,
		/// <summary>
		/// The popup control's border will be sizable and the user will
		/// be able to resize it by grabbing its bottom right corner.
		/// </summary>
		Sizable
	}

	/// <summary>
	/// Specifies the modes in which the AutoComplete control will 
	/// filter the history list for the current text in the target
	/// edit control.
	/// </summary>
	/// <remarks>The <see cref="AutoComplete.MatchMode"/> property uses this
	/// type to fix the type of matching to be performed.</remarks>
	[Serializable]
	public enum AutoCompleteMatchModes 
	{
		/// <summary>
		/// The matching will be automatic. This means that the default
		/// matching routine built into the <see cref="AutoComplete"/> control
		/// will be used.
		/// </summary>
		Automatic =0,
		/// <summary>
		/// The matching will be manual and the user will be able to specify a 
		/// custom event handler to approve a match.
		/// <see cref="AutoComplete.MatchItem"/> event is invoked for each item 
		/// that is in the history list.
		/// </summary>
		Manual
	}
	
	/// <summary>
	/// Specifies the modes of auto completion that can be applied
	/// to an edit control (text box, combo box and controls implementing
	/// IEditControlsEmbed). The <see cref="AutoComplete"/> control provides a AutoComplete
	/// extended property to these types of edit controls. 
	/// 
	/// <seealso cref="IEditControlsEmbed"/>
	/// 
	/// </summary>
	/// <remarks>This type is used by the <see cref="AutoComplete"/> control to set the
	/// extended property on the target controls. The default AutoCompleteMode.Disabled.</remarks>
	public enum AutoCompleteModes
	{
		/// <summary>
		/// AutoComplete will be disabled.
		/// </summary>
		Disabled = 0,
		/// <summary>
		/// Possible matches for the current content of the
		/// active edit control will be presented in the form
		/// of a popup window with a selectable list of matches.
		/// </summary>
		AutoSuggest,
		/// <summary>
		/// The most appropriate match for the current content of
		/// the edit control will be automatically appended to the
		/// edit control and the user can choose to type further or
		/// accept the appended word.
		/// </summary>
		AutoAppend,
		/// <summary>
		/// Activates both AutoAppend and AutoSuggest modes of auto completion
		/// for the target control.
		/// </summary>
		Both,
		/// <summary>
		/// Possible matches from Multiple columns for the current content of the Active edit control
		/// will be presented in a form of a popupwindow with a selectable list of matches.
		/// Multisuggest mode is an extended mode of AutoSuggest.
		/// </summary>
		MultiSuggest,
		/// <summary>
		/// Possible entries of the current content of the Active edit control
		/// will be presented in a form of a popupwindow with a selectable list of matches.
		/// With this mode all possible matches will be highlighted.
		/// </summary>
		MultiSuggestExtended,
	}
}