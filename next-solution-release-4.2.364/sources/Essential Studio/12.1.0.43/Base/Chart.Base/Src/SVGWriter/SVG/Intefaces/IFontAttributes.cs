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

namespace Syncfusion.Windows.Forms.Chart.SvgBase
{
	/// <summary>
	/// Contains the font members.
	/// </summary>
	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IFontAttributes
	{
		/// <summary>
		/// Gets or sets the font family.
		/// </summary>
		/// <value>The font family.</value>
		string FontFamily { get; set; }
		/// <summary>
		/// Gets or sets the font style.
		/// </summary>
		/// <value>The font style.</value>
		EFontStyle FontStyle { get; set; }
		/// <summary>
		/// Gets or sets the font variant.
		/// </summary>
		/// <value>The font variant.</value>
		EFontVariant FontVariant { get; set; }
		/// <summary>
		/// Gets or sets the font weight.
		/// </summary>
		/// <value>The font weight.</value>
		EFontWeight FontWeight { get; set; }
		/// <summary>
		/// Gets or sets the font stretch.
		/// </summary>
		/// <value>The font stretch.</value>
		EFontStretch FontStretch { get; set; }
		/// <summary>
		/// Gets or sets the size of the font.
		/// </summary>
		/// <value>The size of the font.</value>
		Length FontSize { get; set; }
		/// <summary>
		/// Gets or sets the font size adjust.
		/// </summary>
		/// <value>The font size adjust.</value>
		Number FontSizeAdjust { get; set; }
		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font.</value>
		SFont Font { get; set; }
	}
}
