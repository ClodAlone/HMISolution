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
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// Provides designer like context menu support for a PropertyGrid during runtime.
	/// </summary>
	/// <remarks>
	/// <p>This <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/> derived menu has a 
	/// "Reset" menu item, which automatically
	/// provides the property-resetting service when made the context-menu of
	/// a <see cref="System.Windows.Forms.PropertyGrid"/>, during runtime. It also
	/// includes a "Description" menu item to let you show/hide the description portion of the property grid.
	/// Just as in the VS.Net property grid.</p>
	/// <p>
	/// To use this component, just create it passing the PropertyGrid in the constructor.
	/// The context menu will then start appearing for that PropertyGrid. There is no 
	/// design time support for this component.
	/// </p>
	/// </remarks>
	[ToolboxItem(false)]
	public class PropertyGridPopupMenu : PopupMenu
	{
		PropertyGrid propertyGrid;
		BarItem resetItem = null;
		BarItem descriptionVisibilityItem = null;
		PopupMenusManager popupMenusManager = new PopupMenusManager();

		/// <summary>
		/// Creates a new instance of the <b>PropertyGridPopupMenu</b>.
		/// </summary>
		/// <param name="propertyGrid">The <see cref="System.Windows.Forms.PropertyGrid"/> to which
		/// this will be made a context menu.</param>
		public PropertyGridPopupMenu(PropertyGrid propertyGrid)
		{
			this.propertyGrid = propertyGrid;
			this.popupMenusManager.SetXPContextMenu(propertyGrid, this);

			this.ParentBarItem = new ParentBarItem();
			this.ParentBarItem.BeforePopup += new CancelEventHandler(Menu_BeforePopup);

			this.resetItem = new BarItem(SR.GetString(SR.ResetBarItem, this));
			this.descriptionVisibilityItem = new BarItem(SR.GetString(SR.DescriptionBarItemText, this));

			this.ParentBarItem.Items.Add(this.resetItem);
			this.ParentBarItem.Items.Add(this.descriptionVisibilityItem);
			this.ParentBarItem.BeginGroupAt(this.descriptionVisibilityItem);

			this.resetItem.Click += new EventHandler(Reset_Clicked);
			this.descriptionVisibilityItem.Click += new EventHandler(Description_Clicked);
		}

		/// </override>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				this.popupMenusManager.SetXPContextMenu(this.propertyGrid, null);
				this.popupMenusManager = null;
			}
			base.Dispose(disposing);
		}

		private void Reset_Clicked(object sender, EventArgs e)
		{
			this.propertyGrid.ResetSelectedProperty();
		}
		private void Description_Clicked(object sender, EventArgs e)
		{
			this.propertyGrid.HelpVisible = !this.propertyGrid.HelpVisible;
		}
		private void Menu_BeforePopup(object sender, CancelEventArgs e)
		{
			this.resetItem.Enabled = PropertyGridContextMenuHelper.ShouldSerializeSelectedValue(this.propertyGrid);
			this.descriptionVisibilityItem.Checked = this.propertyGrid.HelpVisible;
		}
	}
}