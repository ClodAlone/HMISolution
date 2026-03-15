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
using System.Windows.Forms;
using System.Diagnostics;
using System.Resources;

namespace Syncfusion.Windows.Forms.InternalMenus
{
    /// <exclude/>
    /// <summary>
	/// Implementation class for standard WinForms menu.
	/// </summary>
	internal class WinFormsMenuImp : MenuImp
	{
		public WinFormsMenuImp()
		{
		}

		#region Menu Creation

		#region Factory Methods
		public override object[] CreateMenus(ResourceManager manager, MenuItemStructCollection[] structCollectionArray)
		{
            resourceManager = manager;
			ArrayList menuList = new ArrayList();
			foreach(MenuItemStructCollection collection in structCollectionArray)
			{
				menuList.Add(CreateMenu(collection));
			}
			return (Menu[])menuList.ToArray(typeof(Menu));
		}

		private Menu CreateMenu(MenuItemStructCollection defaultXmlStructCollection)
		{
            MenuItemStructCollection.MenuItemStructEnumerator ienum = defaultXmlStructCollection.GetEnumerator();

			Menu menu = new MainMenu();
			MenuItem.MenuItemCollection mi = new System.Windows.Forms.Menu.MenuItemCollection(menu);

			//add the menu items via the recursive method
			this.CreateMenuItems(mi,ienum);
			
			return menu;
		}
		#endregion

		private void CreateMenuItems(MenuItem.MenuItemCollection collection, MenuItemStructCollection.MenuItemStructEnumerator ienum)
		{
			while(ienum.MoveNext())
			{
				MenuItem mItem = new MenuItem();
				string parent = "";

				mItem.Text = ienum.Current.text;
				
				
				parent = ienum.Current.parent;
				if(ienum.Current.shortcut != null && ienum.Current.shortcut != "")
				{
					mItem.Shortcut = GetShortcutByStringRep(ienum.Current.shortcut);
				}
				if(ienum.Current.eventHandler != null)
				{
					ActionInfo ai = new ActionInfo(MenuLoader.EventActionNamespace + ienum.Current.eventHandler.ToString(),MenuLoader.ParentObject,mItem);
					if(ai.EventHandler != null)
					{
						mItem.Click += ai.EventHandler;
					}
				}
				if(ienum.Current.children != null)
				{
						if(ienum.Current.children.Count > 0)
						{
							CreateMenuItems(mItem.MenuItems, ienum.Current.children.GetEnumerator());
						}
				}
				try
				{
					collection.Add(mItem);
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex.ToString() + "\n\n" + ienum.Current.text);
				}
			}
		}
		#endregion
	}
}
