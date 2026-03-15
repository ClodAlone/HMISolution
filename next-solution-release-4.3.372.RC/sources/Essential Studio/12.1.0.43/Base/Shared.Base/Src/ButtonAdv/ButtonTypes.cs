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

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// The types of buttons that the child button of the
	/// ButtonEdit class can be set to. The ButtonType specifies
	/// the image that is to be used for the button. 
	/// Set the button to normal appearance if you want to use your
	/// own image.
	/// </summary>
	/// <remarks>
	/// The ButtonType and <see cref="Button.Image"/> property are only loosely enforced by the
	/// control and the designer. In the case where the ButtonType is set to one of these
	/// values other than ButtonTypes.Normal and the Image property of the ButtonEditChildButton
	/// is changed, the new image will be displayed and the ButtonType will still be the same
	/// (it will not be changed to normal as its displaying a new image).
	/// <para>
	/// The ButtonTypes are only provided for ease of use and do not in any way change the
	/// functionality of the buttons.
	/// </para>
	/// </remarks>
	[
	Serializable()
	]
	public enum ButtonTypes
	{
		/// <summary>
		/// Normal button. The image can be set by the user.
		/// </summary>
		Normal = 0,
		/// <summary>
		/// Calculator image is used.
		/// </summary>
		Calculator,
		/// <summary>
		/// Currency image is used.
		/// </summary>
		Currency,
		/// <summary>
		/// Down image is used.
		/// </summary>
		Down,
		/// <summary>
		/// Down image like in a Windows XP combo box.
		/// </summary>
		ComboXPDown,
		/// <summary>
		/// Up image is used.
		/// </summary>
		Up,
		/// <summary>
		/// Left image is used.
		/// </summary>
		Left,
		/// <summary>
		/// Right image is used.
		/// </summary>
		Right,
		/// <summary>
		/// Redo image is used.
		/// </summary>
		Redo,
		/// <summary>
		/// Undo image is used.
		/// </summary>
		Undo,
		/// <summary>
		/// Check image is used.
		/// </summary>
		Check,
		/// <summary>
		/// Browse image is used.
		/// </summary>
		Browse,
		/// <summary>
		/// Left end image is used.
		/// </summary>
		LeftEnd,
		/// <summary>
		/// Right end image is used.
		/// </summary>
		RightEnd
	}
}
