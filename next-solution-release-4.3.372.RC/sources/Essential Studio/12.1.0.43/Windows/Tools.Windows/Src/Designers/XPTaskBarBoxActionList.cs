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
	public class XPTaskBarBoxActionList : SyncActionListBase<XPTaskBarBox>
	{
		public XPTaskBarBoxActionList (IComponent component)
			: base(component)
		{
		}

		protected override void InitializeActionList()
		{
			this.AddDesignerActionHeaderItem("Essential Tools - XPTaskBarBox");
			this.AddDesignerActionPropertyItem("Text", "Header Text", "Appearance_", "Specifies the name of the header part of XPTaskBarBox.");
			this.AddDesignerActionMethodItem("AddItem", "Add Item", "Misc", "Adds new XPTaskBarBox");
			this.AddDesignerActionPropertyItem("Items", "Items Collection", "Misc", "Indicates the collection of XPTaskBarBoxes.");

			//Appearance
			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("Collapsed", "Collapsed", "Appearance", "Specifies whether the box should be collapsed.");
			this.AddDesignerActionPropertyItem("ShowCollapseButton", "Show CollapseButton", "Appearance", "Specifies whether the collapse button should be shown.");
			this.AddDesignerActionPropertyItem("HeaderBackColor", "Header BackColor", "Appearance", "Specifies header backcolor of XPTaskBarBox.");
			this.AddDesignerActionPropertyItem("ItemBackColor", "Item BackColor", "Appearance", "Specifies Item backcolor of XPTaskBarBox.");
			this.AddDesignerActionPropertyItem("ImageList", "Image List", "Appearance", "Specifies the imageList.");
			this.AddDesignerActionPropertyItem("PreferredChildPanelHeight", "ChildPanel Height", "Appearance", "Specifies the childPanel height.");
			this.AddDesignerActionPropertyItem("HeaderDirection", "Header Direction", "Appearance", "Specifies the direction of the header.");

			// Behavior
			this.AddDesignerActionHeaderItem("Behavior");
			this.AddDesignerActionPropertyItem("AnimationDelay", "Animation Delay", "Behavior", "Specifies the animation delay during collapse/expand.");
			this.AddDesignerActionPropertyItem("AnimationPositionsCount", "AnimationPositions Count", "Behavior", "Specifies the number of Animations positions during collapse/expand.");
			this.AddDesignerActionPropertyItem("DrawFocusRect", "DrawFocusRect", "Behavior", "Specifies whether focus rectangle should be drawn when clicked.");
			this.AddDesignerActionPropertyItem("ShowToolTip", "ShowToolTip", "Behavior", "Specifies whether tooltips should be shown.");
		}

		public string Text
		{
			get
			{
				string text = string.Empty;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					text = Control.Text;
				}
				return text;
			}
			set
			{
				SetValue("Text", value);
			}
		}

		public Syncfusion.Windows.Forms.Tools.XPTaskBarItemsCollection Items
		{
			get
			{
				Syncfusion.Windows.Forms.Tools.XPTaskBarItemsCollection itemCollection = null;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					itemCollection = Control.Items;
				}
				return itemCollection;
			}
			set
			{
				SetValue("Items", value);
			}
		}

		private const string DEF_ITEMS_PROPERTY_NAME = "Name";
		public void AddItem ( )
		{

			XPTaskBarBox taskBarBox = this.Component as XPTaskBarBox;
			if (taskBarBox != null)
			{
				IDesignerHost host = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (host != null)
				{
					DesignerTransaction transaction = host.CreateTransaction();

					PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(taskBarBox);
					PropertyDescriptor descriptor = properties[DEF_ITEMS_PROPERTY_NAME];

					XPTaskBarItem newItem = new XPTaskBarItem();
					taskBarBox.Items.Add(newItem);

					transaction.Commit();

				}
			}
		}

		#region Appearance
		public bool Collapsed
		{
			get
			{
				bool collapsed = false;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					collapsed = control.Collapsed;
				}
				return collapsed;
			}
			set
			{
				SetValue("Collapsed", value);
			}
		}

		public bool ShowCollapseButton
		{
			get
			{
				bool showButton = true;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					showButton = control.ShowCollapseButton;
				}
				return showButton;
			}
			set
			{
				SetValue("ShowCollapseButton", value);
			}
		}

		public Color HeaderBackColor
		{
			get
			{
				Color headerColor = Color.Empty;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					headerColor = control.HeaderBackColor;
				}
				return headerColor;
			}
			set
			{
				SetValue("HeaderBackColor", value);
			}
		}

		public Color ItemBackColor
		{
			get
			{
				Color color = Color.Empty;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					color = control.ItemBackColor;
				}
				return color;
			}
			set
			{
				SetValue("ItemBackColor", value);
			}
		}

		public ImageList ImageList
		{
			get
			{
				ImageList imageList = null;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					imageList = control.ImageList;
				}
				return imageList;
			}
			set
			{
				SetValue("ImageList", value);
			}
		}

		//public int HeaderImageIndex
		//{
		//    get
		//    {
		//        int index = -1;
		//        if (this.Control != null)
		//        {
		//            XPTaskBarBox control = this.Control as XPTaskBarBox;
		//            index = control.HeaderImageIndex;
		//        }
		//        return index;
		//    }
		//    set
		//    {
		//        SetValue("HeaderImageIndex", value);
		//    }
		//}

		public int PreferredChildPanelHeight
		{
			get
			{
				int height = 30;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					height = control.PreferredChildPanelHeight;
				}
				return height;
			}
			set
			{
				SetValue("PreferredChildPanelHeight", value);
			}
		}

		public Syncfusion.Windows.Forms.Tools.XPTaskBarBox.HeaderDirectionFormat HeaderDirection
		{
			get
			{
				Syncfusion.Windows.Forms.Tools.XPTaskBarBox.HeaderDirectionFormat direction = XPTaskBarBox.HeaderDirectionFormat.LeftToRight;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					direction = control.HeaderDirection;
				}
				return direction;
			}
			set
			{
				SetValue("HeaderDirection", value);
			}
		} 
		#endregion

		#region Behavior
		public bool DrawFocusRect
		{
			get
			{
				bool bDrawFocusRect = true;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					bDrawFocusRect = control.DrawFocusRect;
				}
				return bDrawFocusRect;
			}
			set
			{
				SetValue("DrawFocusRect", value);
			}
		}

		public bool ShowToolTip
		{
			get
			{
				bool showToolTip = false;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					showToolTip = control.ShowToolTip;
				}
				return showToolTip;
			}
			set
			{
				SetValue("ShowToolTip", value);
			}
		}

		public int AnimationDelay
		{
			get
			{
				int delay = 50;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					delay = control.AnimationDelay;
				}
				return delay;
			}
			set
			{
				SetValue("AnimationDelay", value);
			}
		}

		public int AnimationPositionsCount
		{
			get
			{
				int count = 10;
				if (this.Control != null)
				{
					XPTaskBarBox control = this.Control as XPTaskBarBox;
					count = control.AnimationPositionsCount;
				}
				return count;
			}
			set
			{
				SetValue("AnimationPositionsCount", value);
			}
		}


		#endregion
	}
}
#endif