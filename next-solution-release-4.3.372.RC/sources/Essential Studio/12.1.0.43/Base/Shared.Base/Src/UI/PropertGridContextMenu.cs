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
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Provides designer-like context menu support for the PropertyGrid during run-time.
	/// </summary>
	/// <remarks>
	/// <p>This <see cref="System.Windows.Forms.ContextMenu"/> derived menu has a "Reset" menu item, which automatically
	/// provides the property-resetting service with the <b>ContextMenu</b> of
	/// a <see cref="System.Windows.Forms.PropertyGrid"/> during run-time. It also
	/// includes a "Description" menu item to let you show / hide the description portion of the property grid.
	/// Just as in the VS.NET property grid.</p>
	/// <p>
	/// To use this component, just create it passing the PropertyGrid in the constructor.
	/// The context menu will then start appearing for that PropertyGrid. There is no 
	/// design-time support for this component.
	/// </p>
	/// </remarks>
	[ToolboxItem(false)]
	public class PropertyGridContextMenu : ContextMenu
	{
		PropertyGrid propertyGrid;
		MenuItem resetItem = null;
		MenuItem descriptionVisibilityItem = null;

		/// <summary>
		/// Creates a new instance of the <b>PropertyGridContextMenu</b>.
		/// </summary>
		/// <param name="propertyGrid">The <see cref="System.Windows.Forms.PropertyGrid"/> to which
		/// this will be made a context menu.</param>
		public PropertyGridContextMenu(PropertyGrid propertyGrid)
		{
			this.propertyGrid = propertyGrid;
			this.propertyGrid.ContextMenu = this;

			this.resetItem = new MenuItem(SR.GetString(SR.ResetMenuItemText, this));
			this.descriptionVisibilityItem = new MenuItem(SR.GetString(SR.DescriptionMenuItemText,this));

			this.MenuItems.Add(this.resetItem);
			this.MenuItems.Add("-");
			this.MenuItems.Add(this.descriptionVisibilityItem);

			this.resetItem.Click += new EventHandler(Reset_Clicked);
			this.descriptionVisibilityItem.Click += new EventHandler(Description_Clicked);
		}

		/// <override/>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				this.propertyGrid.ContextMenu = null;
			}
			base.Dispose(disposing);
		}

		private void Reset_Clicked(object sender, EventArgs e)
		{
			this.propertyGrid.ResetSelectedProperty();
			this.propertyGrid.Refresh();
		}
		private void Description_Clicked(object sender, EventArgs e)
		{
			this.propertyGrid.HelpVisible = !this.propertyGrid.HelpVisible;
		}
		/// <override/>
		protected override void OnPopup(EventArgs e)
		{
			base.OnPopup(e);
			this.resetItem.Enabled = PropertyGridContextMenuHelper.ShouldSerializeSelectedValue(this.propertyGrid);
			this.descriptionVisibilityItem.Checked = this.propertyGrid.HelpVisible;
		}
	}
}
