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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Security.Permissions;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;

using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Diagnostics;
using Syncfusion.Runtime.InteropServices;	

namespace Syncfusion.Windows.Forms.Tools
{

	/// <summary>
	/// The clipping mode to be used by the control
	/// when returning the text content of the control.
	/// </summary>
	[Serializable()]
	public enum ClipModes
	{
		/// <summary>
		/// Include all literals in the data that is
		/// returned.
		/// </summary>
		IncludeLiterals =0,
		/// <summary>
		/// Exclude all literals in the data that is returned.
		/// </summary>
		ExcludeLiterals
	}

	/// <summary>
	/// Specifies different modes of input 
	/// for the MaskedEditBox control.
	/// </summary>
	[Serializable()]
	public enum MaskInputMode
	{
		/// <summary>
		/// In this input mode, all input is entered in Overtype mode.
		/// The INSERT key will not have any effect in this mode.
		/// </summary>
		OvertypeOnly =0,
		/// <summary>
		/// Operates in Insert mode and when the INSERT key is pressed,
		/// changes to Overtype mode.
		/// </summary>
		Normal
	}

	/// <summary>
	/// The clipping mode to be used by the control
	/// when returning the text content of the control.
	/// </summary>
	public enum MaskedUsageMode
	{
		/// <summary>
		/// Treat as a normal masked edit that does not have any
		/// special constraints with respect to the content.
		/// </summary>
		Normal =0,
		/// <summary>
		/// Treat the contents of the MaskedEditBox as numeric.
		/// </summary>
		Numeric
	}


	/// <summary>
	/// Enumeration for the kind of case sensitivity to be applied
	/// at a particular point of data input.
	/// </summary>
	public enum CasingNormalize
	{
		/// <summary>
		/// Convert to lower case or upper case depending on the mask.
		/// </summary>
		changeToBoth=0,
		/// <summary>
		/// Change to lower case only.
		/// </summary>
		changeToLowerOnly,
		/// <summary>
		/// Change to upper case only.
		/// </summary>
		changeToUpperOnly
	}

	/// <summary>
	/// The various valid masks supported by the control.
	/// </summary>
	public enum MaskCharTypes
	{
		/// <summary>
        /// Digit placeholder '#'. Numeric and white space
		/// </summary>
		maskCharDigitRequired = 0,

		/// <summary>
		/// Decimal placeholder '.' The actual character used is the 
		/// one specified as the decimal placeholder in your 
		/// international settings. This character is treated 
		/// as a literal for masking purposes.
		/// </summary>
		maskCharDecimal,

		/// <summary>
		/// Thousands separator ',' The actual character used is the 
		/// one specified as the thousands separator in your 
		/// international settings. This character is treated as a 
		/// literal for masking purposes.
		/// </summary>
		maskCharThousands,

		/// <summary>
		/// Time separator ':' The actual character used is the one 
		/// specified as the time separator in your international 
		/// settings. This character is treated as a literal for masking 
		/// purposes.
		/// </summary>
		maskCharTimeSep,

		/// <summary>
		/// Date separator '/' The actual character used is the one 
		/// specified as the date separator in your international 
		/// settings. This character is treated as a literal for 
		/// masking purposes.
		/// </summary>
		maskCharDateSep,

		/// <summary>
		/// Escape '\' Treat the next character in the mask string as a literal. 
		/// This allows you to include the '#', &amp;, 'A', and '?' 
		/// characters in the mask. This character is treated as a 
		/// literal for masking purposes.
		/// </summary>
		maskCharEscape,

		/// <summary>
		/// Character placeholder &amp; Valid values for this placeholder 
		/// are ANSI characters in the following ranges: 32-126 and 128-255.
		/// </summary>
		maskCharCharacterRequired,

		/// <summary>
		/// Uppercase &gt; Convert all the characters that follow to uppercase.
		/// </summary>
		maskCharUppercase,

		/// <summary>
		/// Lowercase &lt; Convert all the characters that follow to lowercase.
		/// </summary>
		maskCharLowercase,

		/// <summary>
		/// Alphanumeric character placeholder 'A'
		/// (entry required). For example: a  z, A  Z, or 0  9.
		/// </summary>
		maskCharAplhaNumericRequired,

		/// <summary>
		/// Alphanumeric character placeholder (entry optional)'a'
		/// </summary>
		maskCharAlphaNumericOptional,

		/// <summary>
		/// Digit placeholder (entry optional). For example: 0  9. '9'
		/// </summary>
		maskCharDigitOptional,	

		/// <summary>
		/// Character or space placeholder (entry optional) 'C'
		/// This operates exactly like the &amp; placeholder, and 
		/// ensures compatibility with Microsoft Access.
		/// </summary>
		maskCharCharacterOptional,

		/// <summary>
		/// Letter placeholder. For example: a  z or A  Z '?'
		/// </summary>
		maskCharLetterRequired,

		/// <summary>
		/// Letter placeholder. For example: a  z or A  Z 'y'
		/// </summary>
		maskCharLetterOptional,

		/// <summary>
		/// Hexadecimal placeholder. For example: A9 EF
		/// </summary>
		maskCharHexaDecimalOptional,

		/// <summary>
		/// Hexadecimal placeholder. For example: A9EF
		/// </summary>
		maskCharHexaDecimalRequired
	}

	/// <summary>
	/// Provides the list of modes in which the MaskedEditBox can operate
	/// when it does not have the focus.
	/// </summary>
	public enum PassiveDisplayMode
	{
		/// <summary>
		/// Include all literals in the data that is
		/// returned.
		/// </summary>
		IncludeLiterals =0,
		/// <summary>
		/// Exclude the literal characters.
		/// </summary>
		ExcludeLiterals,
	}

	public enum SpecialCursorPosition
	{
		FirstPosition = 0,
		Decimal,
		FirstMaskPosition
	}

	
	/// <summary>
	/// The clipping mode to be used by the control
	/// when returning the text content of the control.
	/// </summary>
	public enum MaskGroupAlignment
	{
		/// <summary>
		/// Do not apply any alignment.
		/// </summary>
		None =0,
		/// <summary>
		/// Left aligned.
		/// </summary>
		Left,
		/// <summary>
		/// Right aligned.
		/// </summary>
		Right,
		/// <summary>
		/// Center aligned.
		/// </summary>
		Center
	}


}
