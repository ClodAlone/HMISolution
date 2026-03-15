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
    /// Implement this interface if you want to add support for shared scrollbars.
    /// </summary>
    public interface IScrollBarFrame
    {
		/// <summary>
		/// Returns a scrollbar object that implements a horizontal scrollbar.
		/// </summary>
        Control GetHScrollBar(Control control);
		
		/// <summary>
		/// Returns a scrollbar object that implements a vertical scrollbar.
		/// </summary>
		Control GetVScrollBar(Control control);

		/// <summary>
		/// Indicates whether the specified control is activated.
		/// </summary>
		bool IsActive(Control control, ScrollBars sbType);
    }
}
