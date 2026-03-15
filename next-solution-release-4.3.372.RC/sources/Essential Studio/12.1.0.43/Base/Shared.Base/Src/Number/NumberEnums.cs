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

namespace Syncfusion.Windows.Forms.Tools
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Text;
	using System.Globalization;
	using System.Reflection;
	using System.Security.Permissions;
	using System.Runtime.InteropServices;

	using Syncfusion.Windows.Forms.Localization;
	using Syncfusion.Diagnostics;
	using Syncfusion.Runtime.InteropServices;	
	using Syncfusion.ComponentModel;

	/// <summary>
	/// Summary description for SpecialCultureValues.
	/// </summary>
	/// <summary>
	/// Modifier for the currently selected culture.
	/// </summary>
	[Serializable()]
	public enum SpecialCultureValues
	{
		/// <summary>
		/// No modifier for the selected culture.
		/// </summary>
		None =0,
		/// <summary>
		/// Use the current culture on the system.
		/// </summary>
		CurrentCulture,
		/// <summary>
		/// Use the current UI culture.
		/// </summary>
		UICulture,
		/// <summary>
		/// Use the current InstalledCulture.
		/// </summary>
		InstalledCulture
	}


	/// <summary>
	/// Summary description for NumberClipModes.
	/// </summary>
	/// <summary>
	/// The clipping mode to be used by the control
	/// when returning the text content of the control.
	/// </summary>
	[Serializable()]
	public enum NumberClipModes
	{
		/// <summary>
		/// Include all literals in the data that is
		/// returned.
		/// </summary>
		IncludeFormatting =0,
		/// <summary>
		/// Exclude all literals in the data that is returned.
		/// </summary>
		ExcludeFormatting
	}

	/// <summary>
	/// Summary description for CurrencyClipModes.
	/// </summary>
	/// <summary>
	/// The clipping mode to be used by the control
	/// when returning the text content of the control.
	/// </summary>
	[Serializable()]
	public enum CurrencyClipModes
	{
		/// <summary>
		/// Include all literals in the data that is
		/// returned.
		/// </summary>
		IncludeFormatting =0,
		/// <summary>
		/// Exclude all literals in the data that is returned.
		/// </summary>
		ExcludeFormatting
	}

}
