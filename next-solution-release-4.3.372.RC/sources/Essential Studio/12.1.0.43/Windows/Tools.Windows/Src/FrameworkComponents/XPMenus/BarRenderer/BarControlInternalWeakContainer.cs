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
using System.Diagnostics;
using Microsoft.Win32;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Summary description for BarControlInternalWeakContainer.
	/// </summary>
	public class BarControlInternalWeakContainer: WeakReference
	{
		public BarControlInternalWeakContainer(BarControlInternal target) 
			: base(target) 
		{
		}

		public void MenuColorsChangedWeakEventHandler(object sender, EventArgs args)
		{
			BarControlInternal comboDropDown = (BarControlInternal)this.Target;

			if (comboDropDown != null)
			{
				comboDropDown.MenuColorsChanged(sender, args);
			}
			else
			{
				MenuColors.MenuColorsChanged -= new EventHandler(this.MenuColorsChangedWeakEventHandler);
			}
		} 

		public void Office2003ColorsChangedWeakEventHandler(object sender, EventArgs args)
		{
			BarControlInternal comboDropDown = (BarControlInternal)this.Target;

			if (comboDropDown != null)
			{
				comboDropDown.MenuColorsChanged(sender, args);
			}
			else
			{
				Office2003Colors.MenuColorsChanged -= new EventHandler(this.Office2003ColorsChangedWeakEventHandler);
			}
		}

		public void SystemUPChangedWeakEventHandler(object sender, UserPreferenceChangedEventArgs args)
		{
			BarControlInternal comboDropDown = (BarControlInternal)this.Target;

			if (comboDropDown != null)
			{
				comboDropDown.System_UPChanged(sender, args);
			}
			else
			{
				SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.SystemUPChangedWeakEventHandler);
			}
		}
	}
}
