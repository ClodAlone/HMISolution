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
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using System.Collections;

namespace Syncfusion.Windows.Forms.InternalMenus
{

    /// <exclude/>
    internal class ToolBarButtonSeperator : ToolBarButton
	{
		public ToolBarButtonSeperator()
		{
			Style      = ToolBarButtonStyle.Separator;
			Tag   = null;
		}
	}

    /// <exclude/>
    internal class RichToolBarButton : ToolBarButton
	{
		public RichToolBarButton(String text, int imageindex, object evt)
		{
			ToolTipText = text;
			this.ImageIndex = imageindex;
			Tag = evt;
		}
		public RichToolBarButton(String text, int imageindex)
			:this(text,imageindex,null)
		{
			
		}
	}

    /// <exclude/>
    /// <summary>
    ///	Toolbar to be used by default WinForms MenuFactory 
	///</summary>
    internal class StandardToolBar : ToolBar
	{
		
		public StandardToolBar(ResourceManager manager,ToolBarItemStructCollection items)
		{
			ToolTip tip = new ToolTip();
			
			this.Name = items.ToolBarName;
			this.Height = 23;
			this.ButtonSize = new Size(23,23);
			Appearance = ToolBarAppearance.Flat;

			int iconIndex = 0;
			ToolBarItemStructCollection.ToolBarItemStructEnumerator ienum = items.GetEnumerator();
			while(ienum.MoveNext())
			{
				if(ienum.Current.shortcutText == "")
				{
					//separator
					this.Buttons.Add(new ToolBarButtonSeperator());
				}
				else
				{
					iconIndex = Images.ImageCollection.IndexOf(ienum.Current.iconResource,true,manager);
					RichToolBarButton button = new RichToolBarButton(ienum.Current.shortcutText,iconIndex);
					button.Tag = new ActionInfo(MenuLoader.EventActionNamespace + ienum.Current.eventHandler,MenuLoader.ParentObject,button);
					switch(ienum.Current.style.Trim().ToLower())
					{
						case "togglebutton":
							button.Style = ToolBarButtonStyle.ToggleButton;
							break;
						default:
							button.Style = ToolBarButtonStyle.PushButton;
							break;
					}
					Buttons.Add(button);
				}
			}
			this.ImageList = Images.CurrentImageList;

			ButtonClick += new ToolBarButtonClickEventHandler(ToolBarClickEvent);
		}
		
		void ToolBarClickEvent(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button.Tag != null) {
				((ActionInfo)e.Button.Tag).EventHandler.DynamicInvoke(new object[]{sender,e});
			}
		}
	}
}
