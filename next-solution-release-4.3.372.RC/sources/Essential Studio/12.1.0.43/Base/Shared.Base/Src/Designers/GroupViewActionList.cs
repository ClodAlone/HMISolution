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

namespace Syncfusion.Windows.Forms.Tools
{
	public class GroupViewActionList : SyncActionListBase<GroupView>
	{

		public GroupViewActionList (IComponent component)
			: base(component)
		{
			
		}

		protected override void InitializeActionList ( )
		{
			this.AddDesignerActionHeaderItem("Essential Tools - GroupView");

			this.AddDesignerActionPropertyItem("Name", "Name", "Misc", "Specifies the name of the control.");
			this.AddDesignerActionPropertyItem("GroupViewItems", "Items Collection", "Misc", "Specifies the collection of groupViewItems.");			

			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("ButtonView", "Button View", "Appearance", "Specifies whether Button view should be enabled.");
			this.AddDesignerActionPropertyItem("FlatLook", "Flat Look", "Appearance", "Specifies whether control should be displayed with flat look.");
			this.AddDesignerActionPropertyItem("FlowView", "Flow View", "Appearance", "Specifies whether control should use flow type display.");
			this.AddDesignerActionPropertyItem("SmallImageView", "Small Image View", "Appearance", "Specifies whether control should use small images for items.");
			this.AddDesignerActionPropertyItem("ThemesEnabled", "Themes Enabled", "Appearance", "Specifies whether themes should be enabled.");

			this.AddDesignerActionHeaderItem("Behavior");
			this.AddDesignerActionPropertyItem("LargeImageList", "Large Imagelist", "Behavior", "Specifies the imagelist used for large images.");
			this.AddDesignerActionPropertyItem("SmallImageList", "Small Imagelist", "Behavior", "Specifies the imagelist used for small images.");
			this.AddDesignerActionPropertyItem("AllowDragDrop", "Allow Drag Drop", "Behavior", "Specifies whether drag drop should be enabled.");
			this.AddDesignerActionPropertyItem("HighlightImage", "Highlight Image", "Behavior", "Specifies whether item's image should be highlighted when mouse hover.");
			this.AddDesignerActionPropertyItem("HighlightText", "Highlight Text", "Behavior", "Specifies whether item's text should be highlighted when mouse hover.");
			this.AddDesignerActionPropertyItem("IntegratedScrolling", "Integrated Scrolling", "Behavior", "Specifies whether control use scrolling option integrated with the parent control.");
			this.AddDesignerActionPropertyItem("TextWrap", "Wrap Text", "Behavior", "Specfies whether item's text should be wrapped.");
		}

		public Syncfusion.Windows.Forms.Tools.GroupView.GroupViewItemCollection GroupViewItems
		{
			get
			{
				Syncfusion.Windows.Forms.Tools.GroupView.GroupViewItemCollection items = null;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					items = control.GroupViewItems;
				}
				return items;
			}
			set
			{
				SetValue("GroupViewItems", value);
			}
		}

		public string Name
		{
			get
			{
				string name = string.Empty;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					name = control.Name;
				}
				return name;
			}
			set
			{
				SetValue("Name", value);
			}
		}

		public bool ButtonView
		{
			get
			{
				bool buttonView = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					buttonView = control.ButtonView;
				}
				return buttonView;
			}
			set
			{
				SetValue("ButtonView", value);
			}
		}

		public bool FlatLook
		{
			get
			{
				bool flatLook = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					flatLook = control.FlatLook;
				}
				return flatLook;
			}
			set
			{
				SetValue("FlatLook", value);
			}
		}

		public bool FlowView
		{
			get
			{
				bool flowView = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					flowView = control.FlowView;
				}
				return flowView;
			}
			set
			{
				SetValue("FlowView", value);
			}
		}

		public bool SmallImageView
		{
			get
			{
				bool smallImageView = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					smallImageView = control.SmallImageView;
				}
				return smallImageView;
			}
			set
			{
				SetValue("SmallImageView", value);
			}
		}

		public bool ThemesEnabled
		{
			get
			{
				bool themesEnabled = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					themesEnabled = control.ThemesEnabled;
				}
				return themesEnabled;
			}
			set
			{
				SetValue("ThemesEnabled", value);
			}
		}

		public ImageList LargeImageList
		{
			get
			{
				ImageList largeImageList = null;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					largeImageList = control.LargeImageList;
				}
				return largeImageList;
			}
			set
			{
				SetValue("LargeImageList", value);
			}
		}

		public ImageList SmallImageList
		{
			get
			{
				ImageList smallImagelist = null;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					smallImagelist = control.SmallImageList;
				}
				return smallImagelist;
			}
			set
			{
				SetValue("SmallImageList", value);
			}
		}

		public bool AllowDragDrop
		{
			get
			{
				bool allowDragDrop = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					allowDragDrop = control.AllowDragDrop;
				}
				return allowDragDrop;
			}
			set
			{
				SetValue("AllowDragDrop", value);
			}
		}

		public bool HighlightImage
		{
			get
			{
				bool highlightImage = true;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					highlightImage = control.HighlightImage;
				}
				return highlightImage;
			}
			set
			{
				SetValue("HighlightImage", value);
			}
		}

		public bool HighlightText
		{
			get
			{
				bool highlightText = true;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					highlightText = control.HighlightText;
				}
				return highlightText;
			}
			set
			{
				SetValue("HighlightText", value);
			}
		}

		public bool IntegratedScrolling
		{
			get
			{
				bool scrolling = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					scrolling = control.IntegratedScrolling;
				}
				return scrolling;
			}
			set
			{
				SetValue("IntegratedScrolling", value);
			}
		}

		public bool TextWrap
		{
			get
			{
				bool textWrap = false;
				if (this.Control != null)
				{
					GroupView control = this.Control as GroupView;
					textWrap = control.TextWrap;
				}
				return textWrap;
			}
			set
			{
				SetValue("TextWrap", value);
			}
		}		
	}
}
#endif
