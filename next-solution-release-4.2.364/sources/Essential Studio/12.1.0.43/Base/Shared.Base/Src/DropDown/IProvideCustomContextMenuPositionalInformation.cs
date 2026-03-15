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
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Reflection;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Implement this interface to provide context menu location in your control when 
	/// the context menu is invoked by a keyboard key.
	/// </summary>
	/// <remarks>Used by the XPMenus framework (when using PopupMenus in Essential Tools) to determine the context menu location
	/// for a control.</remarks>
	public interface IProvideCustomContextMenuPositionalInformation
	{
		/// <summary>
		/// Returns a point in client coordinates of the control.
		/// </summary>
		/// <remarks>
		/// <para>This method will be called when the context menu is being
		/// invoked due to a key like Shift + F10. The control that implements this
		/// interface should then return a point in client coordinates of the
		/// control where the context menus should be shown. </para>
		/// <para>If the control were a
		/// TreeView for example, it should then return a location beside the current
		/// selected node.</para>
		/// </remarks>
		Point GetMenuPositionForKeyboardInvoke();
	}
	public class PropertyGridContextMenuHelper
	{
		public static bool ShouldSerializeSelectedValue(PropertyGrid pg)
		{
			// Doing a ((GridEntry)pg.SelectedGridItem).ShouldSerializeSelectedValue()

			if(pg.SelectedGridItem == null)
				return false;

			GridItem gi = pg.SelectedGridItem as GridItem;

			//Assembly formsAssembly = Assembly.LoadWithPartialName("System.Windows.Forms");
			Type gridEntryType = Type.GetType("System.Windows.Forms.PropertyGridInternal.GridEntry, System.Windows.Forms");

			if(gridEntryType != null)
			{
				MethodInfo mInfo = gridEntryType.GetMethod("ShouldSerializePropertyValue", 
					BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic);
				if(mInfo != null)
				{
					return (bool)mInfo.Invoke(gi, new object[]{});
				}
			}
			return true;
		}
	}
}