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
using System.ComponentModel;

namespace Syncfusion.Windows.Forms
{
    /// <summary></summary>
    public enum FontSize
    {
        /// <summary></summary>
        Normal = 0,
        /// <summary></summary>
        Large = 1
    }

    /// <summary></summary>
    public enum UseStyle
    {
        /// <summary>
        /// UseVisualStyle is true
        /// </summary>
        True = 1,
        /// <summary>
        /// UseVisualStyle is false
        /// </summary>
        False = 2,
        /// <summary>
        /// value for UseVisualStyle inherits from parent
        /// </summary>
        [ Browsable( false ) ]
        Inherited = 0
    }

	/// <summary>
	/// Specifies the appearance of the button.
	/// </summary>
	public enum ButtonAppearance
	{
        /// <summary></summary>
        [ Browsable( false ) ]
        None = 0,
        /// <summary></summary>
		Classic = 1,
		/// <summary></summary>
		Office2000 = 2,
		/// <summary></summary>
		WindowsXP = 3,
		/// <summary></summary>
		OfficeXP = 4,
		/// <summary></summary>
		Office2003 = 5,
		/// <summary></summary>
        Office2007 = 6,
        /// <summary></summary>
		Office2010 = 7,
        /// <summary></summary>
        Metro =8
	}

	/// <summary>
	/// Specifies the state of the button.
	/// </summary>
	[ Flags ]
	public enum ButtonAdvState
	{
		/// <summary></summary>
		Default = 1,
		/// <summary></summary>
		MouseOver = 2,
		/// <summary></summary>
		Pressed = 4,
        /// <summary></summary>
        Checked = 8,
        /// <summary></summary>
        Flat = 16,
        /// <summary></summary>
        Inactive = 32,
        /// <summary></summary>
        All
	}

	/// <summary>
	/// Specifies ButtonAdv border style.
	/// </summary>
	public enum ButtonAdvBorderStyle
	{
		/// <summary></summary>
		None,
		/// <summary></summary>
		Default,
		/// <summary></summary>
		Dashed,
		/// <summary></summary>
		Dotted,
		/// <summary></summary>
		Inset,
		/// <summary></summary>
		Outset,
		/// <summary></summary>
		Solid,
		/// <summary></summary>
		Bump,
		/// <summary></summary>
		Etched,
		/// <summary></summary>
		Flat,
		/// <summary></summary>
		Raised,
		/// <summary></summary>
		RaisedInner,
		/// <summary></summary>
		RaisedOuter,
		/// <summary></summary>
		Sunken,
		/// <summary></summary>
		SunkenInner,
		/// <summary></summary>
		SunkenOuter
	}
}