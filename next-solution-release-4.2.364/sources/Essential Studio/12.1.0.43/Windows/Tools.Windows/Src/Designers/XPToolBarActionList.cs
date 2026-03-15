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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Text;
using System.Reflection;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Design;
using System.Globalization;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.XPMenus;

namespace Syncfusion.Windows.Forms.Tools
{
	public class XPToolBarActionList : SyncActionListBase<XPToolBar>
	{

		public XPToolBarActionList (IComponent component)
			: base(component)
		{

		}

		protected override void InitializeActionList ( )
		{
			this.AddDesignerActionHeaderItem("Essential Tools - XPToolBar");

			// Appearance
			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("Style", "Style", "Appearance", "Specifies the VisualStyle to be used.");
			this.AddDesignerActionPropertyItem("Items", "Items Collection", "Appearance", "Specfies the collection of barItems");
			this.AddDesignerActionPropertyItem("LargeIcons", "Large Icons", "Appearance", "Specifies whether large icons should be used.");
			this.AddDesignerActionPropertyItem("ThemesEnabled", "ThemesEnabled", "Appearance", "Specifies whether themes should be enabled.");

			// Misc
			this.AddDesignerActionHeaderItem("Misc");
			this.AddDesignerActionPropertyItem("ShowChevron", "Show Chevron", "Misc", "Specifies whether chevron button should be visible or not.");

			// Layout
			this.AddDesignerActionHeaderItem("Layout");
			this.AddDesignerActionPropertyItem("Dock", "Dock", "Layout", "Defines which borders of the control are to be bound to the container.");
		}

		public Syncfusion.Windows.Forms.Tools.XPMenus.BarItems Items
		{
			get
			{
				Syncfusion.Windows.Forms.Tools.XPMenus.BarItems items = null;
				if (this.Control != null)
				{
					XPToolBar control = this.Control as XPToolBar;
					items = control.Items;
				}
				return items;
			}
			set
			{
				SetValue("Items", value);
			}
		}

		public bool LargeIcons
		{
			get
			{
				bool largeIcons = false;
				if (this.Control != null)
				{
					XPToolBar control = this.Control as XPToolBar;
					largeIcons = control.LargeIcons;
					
				}
				return largeIcons;
			}
			set
			{
				SetValue("LargeIcons", value);
			}
		}

		public bool ThemesEnabled
		{
			get
			{
				bool themesEnabled = false;
				if (this.Control != null)
				{
					XPToolBar control = this.Control as XPToolBar;
					themesEnabled = control.ThemesEnabled;
				}
				return themesEnabled;
			}
			set
			{
				SetValue("ThemesEnabled", value);
			}
		}

		public VisualStyle Style
		{
			get
			{
				VisualStyle style = VisualStyle.OfficeXP;
				if (this.Control != null)
				{
					XPToolBar control = this.Control as XPToolBar;
					style = control.Style;
				}
				return style;
			}
			set
			{
				SetValue("Style", value);
			}
		}

		public bool ShowChevron
		{
			get
			{
				bool showChevron = false;
				if (this.Control != null)
				{
					XPToolBar control = this.Control as XPToolBar;
					showChevron = control.ShowChevron;
				}
				return showChevron;
			}
			set
			{
				SetValue("ShowChevron", value);
			}
		}

		public DockStyle Dock
		{

			get
			{
				DockStyle dock = DockStyle.None;
				if (this.Control != null)
				{
					XPToolBar control = this.Control as XPToolBar;
					dock = control.Dock;
				}
				return dock;
			}
			set
			{
				SetValue("Dock", value);
			}

		}
	}
}
#endif
