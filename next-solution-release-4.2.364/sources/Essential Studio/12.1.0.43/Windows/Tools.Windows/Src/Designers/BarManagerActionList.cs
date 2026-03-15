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
	public class BarManagerActionList : SyncActionListBase<BarManager>
	{
		public BarManagerActionList (IComponent component)
			: base(component)
		{

		}

		protected override void InitializeActionList ( )
		{
			if(this.Component is MainFrameBarManager)
				this.AddDesignerActionHeaderItem("Essential Tools - MainFrameBarManager");
			else if(this.Component is ChildFrameBarManager)
				this.AddDesignerActionHeaderItem("Essential Tools - ChildFrameBarManager");

			// Misc
			this.AddDesignerActionHeaderItem("Customization");
			this.AddDesignerActionMethodItem("Customize", "Customize...", "Misc", "Customize Menus Dialog.");
			if (this.Component is MainFrameBarManager)
			{
				this.AddDesignerActionMethodItem("AddDetachedCommandBar", "Add Detached CommandBar", "Misc", "Adds a CommandBar.");
				this.AddDesignerActionMethodItem("AddDetachedControlBar", "Add Detached ControlBar", "Misc", "Adds a ControlBar.");
			}

			// Appearance
			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("Style", "Style", "Appearance", "Specifies the style to be used for menus.");
			this.AddDesignerActionPropertyItem("ImageList", "ImageList", "Appearance", "Specifies the ImageList used when in small icons mode.");
			this.AddDesignerActionPropertyItem("ImageListAdv", "ImageListAdv", "Appearance", "Specifies the ImageListAdv used when in small icons mode.");
			this.AddDesignerActionPropertyItem("LargeImageList", "LargeImageList", "Appearance", "Specifies the ImageList used when in large icons mode");
			this.AddDesignerActionPropertyItem("LargeImageListAdv", "LargeImageListAdv", "Appearance", "Specifies the ImageListAdv used when in large icons mode");
			
			if (this.Component is MainFrameBarManager)
			{
				this.AddDesignerActionPropertyItem("LargeIcons", "LargeIcons", "Appearance", "Specifies whether large icons should be used");
				this.AddDesignerActionPropertyItem("ThemesEnabled", "ThemesEnabled", "Appearance", "Specifies whether themes should be enabled.");

				// Persist
				this.AddDesignerActionHeaderItem("Persistance");
				this.AddDesignerActionPropertyItem("AutoLoadToolBarPositions", "AutoLoadToolBarPositions", "Persistance", "Specifies whether the persisted toolbars positions should be loaded automatically.");
				this.AddDesignerActionPropertyItem("AutoPersistCustomization", "AutoPersistCustomization", "Persistance", "Specifies whether the toolBars should be persisted when customized.");
			}
		}

		public void Customize ( )
		{
			if (this.Component != null)
			{
				BarManager barManager = this.Component as BarManager;
				barManager.Customize((System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)));
			}
		}

		internal IDesignerHost iDesignerHost;
		
		public void AddDetachedControlBar ( )
		{
			iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			ControlBar newControlBar = (ControlBar)this.iDesignerHost.CreateComponent(typeof(ControlBar));
			if (this.Component is MainFrameBarManager)
			{
				MainFrameBarManager manager = this.Component as MainFrameBarManager;
				manager.DetachedCommandBars.Add(newControlBar);
			}
			
		}

		public void AddDetachedCommandBar ( )
		{
			iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
			CommandBar newCommandBar = (CommandBar)this.iDesignerHost.CreateComponent(typeof(CommandBar));
			if (this.Component is MainFrameBarManager)
			{
				MainFrameBarManager manager = this.Component as MainFrameBarManager;
				manager.DetachedCommandBars.Add(newCommandBar);
			}
		}

		public bool AutoLoadToolBarPositions
		{
			get
			{
				bool autoLoadPositions = true;
				if (this.Component != null)
				{
					if (this.Component is MainFrameBarManager)
					{
						MainFrameBarManager barManagerComponent = this.Component as MainFrameBarManager;
						autoLoadPositions = barManagerComponent.AutoLoadToolBarPositions;
					}
				}
				return autoLoadPositions;
			}
			set
			{
				SetValue("AutoLoadToolBarPositions", value);
			}
		}

		public bool AutoPersistCustomization
		{
			get
			{
				bool persistCustomization = true;
				if (this.Component != null)
				{
					if (this.Component is MainFrameBarManager)
					{
						MainFrameBarManager barManagerComponent = this.Component as MainFrameBarManager;
						persistCustomization = barManagerComponent.AutoPersistCustomization;
					}
					
				}
				return persistCustomization;
			}
			set
			{
				SetValue("AutoPersistCustomization", value);
			}
		}

		public VisualStyle Style
		{
			get
			{
				VisualStyle style = VisualStyle.OfficeXP;
				if (this.Component != null)
				{
					if (this.Component is MainFrameBarManager)
					{
						MainFrameBarManager barManagerComponent = this.Component as MainFrameBarManager;
						style = barManagerComponent.Style;
					}
					else if(this.Component is ChildFrameBarManager)
					{
						ChildFrameBarManager barManagerComponent = this.Component as ChildFrameBarManager;
						style = barManagerComponent.Style;
					}
				}
				return style;
			}
			set
			{
				SetValue("Style", value);
			}
		}

		#region ImageList
		/// <summary>
		/// 
		/// </summary>
		public ImageList ImageList
		{
			get
			{
				BarManager barManager = this.Component as BarManager;

				if (barManager != null)
				{
					return barManager.ImageList;
				}

				return null;
			}
			set
			{
				SetValue("ImageList", value);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public ImageListAdv ImageListAdv
		{
			get
			{
				BarManager barManager = this.Component as BarManager;

				if (barManager != null)
				{
					return barManager.ImageListAdv;
				}

				return null;
			}
			set
			{
				SetValue("ImageListAdv", value);
			}
		}

		#endregion

		public ImageList LargeImageList
		{
			get
			{
				BarManager barManager = this.Component as BarManager;

				if (barManager!=null)
				{
					return barManager.LargeImageList;
				}
				
				return null;
			}
			set
			{
				SetValue("LargeImageList", value);
			}
		}
	
		public ImageListAdv LargeImageListAdv
		{
			get
			{
				BarManager barManager = this.Component as BarManager;

				if (barManager != null)
				{
					return barManager.LargeImageListAdv;
				}

				return null;
			}
			set
			{
				SetValue("LargeImageListAdv", value);
			}
		}

		public bool LargeIcons
		{
			get
			{
				bool largeIcons = false;
				if (this.Component != null)
				{
					if (this.Component is MainFrameBarManager)
					{
						MainFrameBarManager barManagerComponent = this.Component as MainFrameBarManager;
						largeIcons = barManagerComponent.LargeIcons;
					}
					else if (this.Component is ChildFrameBarManager)
					{
						ChildFrameBarManager barManagerComponent = this.Component as ChildFrameBarManager;
						largeIcons = barManagerComponent.LargeIcons;
					}
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
				if (this.Component != null)
				{
					if (this.Component is MainFrameBarManager)
					{
						MainFrameBarManager barManagerComponent = this.Component as MainFrameBarManager;
						themesEnabled = barManagerComponent.ThemesEnabled;
					}
				}
				return themesEnabled;
			}
			set
			{
				SetValue("ThemesEnabled", value);
			}
		}

		
	}
}
#endif
