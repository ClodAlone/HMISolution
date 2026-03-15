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
	public class AutoCompleteActionList : SyncActionListBase<AutoComplete>
	{
		public AutoCompleteActionList (IComponent component)
			: base(component)
		{

		}

		protected override void InitializeActionList ( )
		{
			this.AddDesignerActionHeaderItem("Essential Tools - AutoComplete");
			this.AddDesignerActionHeaderItem("Appearance");
			this.AddDesignerActionPropertyItem("ImageList", "ImageList", "Appearance", "Specifies the imagelist used by the control.");
			this.AddDesignerActionPropertyItem("AdjustHeightToItemCount", "AdjustHeightToItemCount", "Appearance", "Specifies if the height of the dropdown should be adjusted based on the number of items.");
			this.AddDesignerActionPropertyItem("ShowColumnHeader", "ShowColumnHeader", "Appearance", "Specifies if dropdown list of possible matches will show their headers.");

			this.AddDesignerActionHeaderItem("Behavior");
			this.AddDesignerActionPropertyItem("AutoAddItem", "AutoAddItem", "Behavior", "Specifies if user wants to add items to the history.");
			this.AddDesignerActionPropertyItem("AutoPersistentDropDownSize", "AutoPersistentDropDownSize", "Behavior", "Indicates whether the dropdown size is persistent.");

			this.AddDesignerActionHeaderItem("Misc");
			this.AddDesignerActionPropertyItem("ShowCloseButton", "ShowCloseButton", "Misc", "");
			this.AddDesignerActionPropertyItem("ShowGripper", "ShowGripper", "Misc", "");

		}

		public ImageList ImageList
		{
			get
			{
				ImageList imageList = null;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					imageList = autoComplete.ImageList;
				}
				return imageList;
			}
			set
			{
				SetValue("ImageList", value);
			}
		}

		public bool AdjustHeightToItemCount
		{
			get
			{
				bool adjustHeightToItemCount = true;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					adjustHeightToItemCount = autoComplete.AdjustHeightToItemCount;
				}
				return adjustHeightToItemCount;
			}
			set
			{
				SetValue("AdjustHeightToItemCount", value);
			}
		}

		public bool ShowColumnHeader
		{
			get
			{
				bool showColumnHeader = false;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					showColumnHeader = autoComplete.ShowColumnHeader;
				}
				return showColumnHeader;
			}
			set
			{
				SetValue("ShowColumnHeader", value);
			}
		}

		public bool AutoAddItem
		{
			get
			{
				bool autoAddItem = false;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					autoAddItem = autoComplete.AutoAddItem;
				}
				return autoAddItem;
			}
			set
			{
				SetValue("AutoAddItem", value);
			}
		}

		public bool AutoPersistentDropDownSize
		{
			get
			{
				bool autoPersistentDropDownSize = false;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					autoPersistentDropDownSize = autoComplete.AutoPersistentDropDownSize;
				}
				return autoPersistentDropDownSize;
			}
			set
			{
				SetValue("AutoPersistentDropDownSize", value);
			}
		}

		public bool ShowCloseButton
		{
			get
			{
				bool showCloseButton = true;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					showCloseButton = autoComplete.ShowCloseButton;
				}
				return showCloseButton;
			}
			set
			{
				SetValue("ShowCloseButton", value);
			}
		}

		public bool ShowGripper
		{
			get
			{
				bool showGripper = true;
				if (this.Component != null)
				{
					AutoComplete autoComplete = this.Component as AutoComplete;
					showGripper = autoComplete.ShowGripper;
				}
				return showGripper;
			}
			set
			{
				SetValue("ShowGripper", value);
			}
		}
	}
}
#endif