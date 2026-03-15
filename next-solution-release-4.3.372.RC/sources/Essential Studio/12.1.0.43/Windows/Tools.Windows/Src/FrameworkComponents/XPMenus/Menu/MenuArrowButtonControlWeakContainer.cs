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

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Summary description for MenuGridControlBaseWeakContainer.
	/// </summary>
	public class MenuGridControlBaseWeakContainer: WeakReference
	{
		public MenuGridControlBaseWeakContainer( MenuGridControlBase target )
			: base( target )
		{
		}

		public void MenuColorsChangedWeakEventHandler( object sender, EventArgs args )
		{
			MenuGridControlBase comboDropDown = (MenuGridControlBase)this.Target;

			if( comboDropDown != null )
			{
				comboDropDown.MenuColorsChanged( sender, args );
			}
			else
			{
				MenuColors.MenuColorsChanged -= new EventHandler( this.MenuColorsChangedWeakEventHandler );
			}
		}

		public void Office2003ColorsChangedWeakEventHandler( object sender, EventArgs args )
		{
			MenuGridControlBase comboDropDown = (MenuGridControlBase)this.Target;

			if( comboDropDown != null )
			{
				comboDropDown.MenuColorsChanged( sender, args );
			}
			else
			{
				Office2003Colors.MenuColorsChanged -= new EventHandler( this.Office2003ColorsChangedWeakEventHandler );
			}
		}
	}
}
