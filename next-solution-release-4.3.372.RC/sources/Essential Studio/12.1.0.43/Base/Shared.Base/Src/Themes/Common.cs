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
	/// <summary>
	/// Specifies the style with which some controls will appear and behave.
	/// </summary>
	/// <remarks>
	/// Every control that incorporates these styles will define the styles differently, 
	/// so take a look at the individual control for more information.
	/// </remarks>
	public enum VisualStyle
	{
		/// <summary>
		/// Classic appearance.
		/// </summary>
		Default,
		/// <summary>
		/// Office XP-like appearance.
		/// </summary>
		OfficeXP,
		/// <summary>
		/// Office 2003-like appearance.
		/// </summary>
		Office2003,
		/// <summary>
		/// Visual Studio 2005-like appearance.
		/// </summary>
		VS2005,
		/// <summary>
		/// Office 2007-like appearance.
		/// </summary>
		Office2007,
		/// <summary>
		/// Office 2007 Outlook-like appearance.
		/// </summary>
		Office2007Outlook,

        /// <summary>
        /// Office 2010-like appearance.
        /// </summary>
        Office2010,
        /// <summary>
        /// Visual Studio 2010-like appearance.
        /// </summary>

        VS2010,

        /// <summary>
        /// Metro-like appearance.
        /// </summary>
        Metro
       
	}
}