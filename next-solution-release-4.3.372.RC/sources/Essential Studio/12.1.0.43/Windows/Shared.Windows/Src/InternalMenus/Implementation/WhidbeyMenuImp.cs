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
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
	internal class WhidbeyMenuImp: MenuImp
	{
		#region Menu Creation
		#region Factory Methods
		public override object[] CreateMenus( ResourceManager manager, MenuItemStructCollection[] structCollectionArray )
		{
			resourceManager = manager;
			ArrayList menuList = new ArrayList();
			foreach( MenuItemStructCollection collection in structCollectionArray )
			{
				menuList.Add( CreateMenu( collection ) );
			}
			return (MenuStrip[])menuList.ToArray( typeof( MenuStrip ) );
		}

		private MenuStrip CreateMenu( MenuItemStructCollection defaultXmlStructCollection )
		{
			MenuItemStructCollection.MenuItemStructEnumerator ienum = defaultXmlStructCollection.GetEnumerator();// this.MenuItemCollection.GetEnumerator();

			MenuStrip menu = new MenuStrip();

			this.CreateMenuItems( menu.Items, ienum );

			return menu;
		}
		#endregion

		private void CreateMenuItems( ToolStripItemCollection items, MenuItemStructCollection.MenuItemStructEnumerator ienum )
		{
			while( ienum.MoveNext() )
			{
				ToolStripItem mItem;

				if( ienum.Current.text != null && (ienum.Current.text.Trim() == "" || ienum.Current.text.Trim() == "-") )
				{
					mItem = new ToolStripSeparator();
				}
				else
				{
					mItem = new ToolStripMenuItem();

					mItem.Text = ienum.Current.text;

					if( ienum.Current.iconResource != null )
					{
						int imageIndex = Images.ImageCollection.IndexOf( ienum.Current.iconResource, true, resourceManager );
						mItem.Image = Images.ImageCollection[imageIndex].bmp;
					}
					if( ienum.Current.eventHandler != null )
					{
						ActionInfo ai = new ActionInfo( MenuLoader.EventActionNamespace + ienum.Current.eventHandler.ToString(), MenuLoader.ParentObject, mItem );
						if( ai.EventHandler != null )
						{
							mItem.Click += ai.EventHandler;
						}
					}
					if( ienum.Current.children != null )
					{
						if( ienum.Current.children.Count > 0 )
						{
							CreateMenuItems( ((ToolStripMenuItem)mItem).DropDownItems, ienum.Current.children.GetEnumerator() );
						}
					}
				}
				try
				{
					items.Add( mItem );
				}
				catch( Exception ex )
				{
					Trace.WriteLine( ex.ToString() + "\n\n" + ienum.Current.text );
				}
			}
		}
		#endregion
	}
#endif
}
