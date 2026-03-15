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
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Styles
{
	/// <summary>
	/// StyleInfoPropertyGrid is a <see cref="PropertyGrid"/> that will reset a specific property when the user right-clicks on the item.
	/// </summary>
	[
	ToolboxItem(false),
	]
	public class StyleInfoPropertyGrid : PropertyGrid
	{
		internal const int WM_CONTEXTMENU = 123; // 0x007b 

		/// <override/>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
		protected override/*ContainerControl*/ void WndProc(ref Message m)  
		{
			// TODO: Display a drop-down context menu here that offers a "Reset" action.
			if (m.Msg == WM_CONTEXTMENU) 
			{
				m.Result = IntPtr.Zero;
				ResetSelectedProperty();
				/*
				//this.ContextMenu.Show(this, new Point(OGUtil.SignedLOWORD(m.LParam), OGUtil.SignedHIWORD(m.LParam)));
				GridItem item = this.SelectedGridItem;
				if (item != null)
				{
					PropertyDescriptor pd = item.PropertyDescriptor;
					if (pd != null)
					{
						object parent = PropertyDescriptor
						if (parent != null && pd.ShouldSerializeValue(item.Value))
							pd.ResetValue(item.Value);
					}
				}*/
			}
			else
				base.WndProc(ref m);
		}


	}
}
