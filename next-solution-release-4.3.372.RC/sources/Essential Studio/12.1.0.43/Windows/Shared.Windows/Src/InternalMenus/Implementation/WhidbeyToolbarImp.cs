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
using System.Resources;
using System.Collections;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.InternalMenus
{
#if !(SyncfusionFramework1_0 || SyncfusionFramework1_1)
	internal class WhidbeyToolBarImp: ToolBarImp
	{

		public WhidbeyToolBarImp()
		{

		}

		#region Factory Methods for Toolbar Creation
		public override object[] CreateToolBars( ResourceManager manager, ToolBarItemStructCollection[] structCollectionArray )
		{
			resourceManager = manager;
			ArrayList toolbarList = new ArrayList();
			foreach( ToolBarItemStructCollection collection in structCollectionArray )
			{
				toolbarList.Add( CreateToolBar( collection ) );
			}
			return (ToolStrip[])toolbarList.ToArray( typeof( ToolStrip ) );
		}

		private ToolStrip CreateToolBar( ToolBarItemStructCollection defaultToolBarStructCollection )
		{

			ToolStrip toolStrip = new ToolStrip();

			CreateToolBarItems( toolStrip, defaultToolBarStructCollection );
			return toolStrip;
		}
		private void CreateToolBarItems( ToolStrip toolStrip, ToolBarItemStructCollection items )
		{
			ToolTip tip = new ToolTip();

			toolStrip.Name = items.ToolBarName;
			//this.Name = items.ToolBarName;
			//this.Height = 23;
			//this.ButtonSize = new Size(23, 23);
			//Appearance = ToolBarAppearance.Flat;

			int iconIndex = 0;
			ToolBarItemStructCollection.ToolBarItemStructEnumerator ienum = items.GetEnumerator();
			while( ienum.MoveNext() )
			{
				if( ienum.Current.shortcutText == "" )
				{
					//separator
					toolStrip.Items.Add( new ToolStripSeparator() );
				}
				else
				{
					iconIndex = Images.ImageCollection.IndexOf( ienum.Current.iconResource, true, resourceManager );
					ToolStripButton button = new ToolStripButton( Images.ImageCollection[iconIndex].bmp );
					button.Tag = new ActionInfo( MenuLoader.EventActionNamespace + ienum.Current.eventHandler, MenuLoader.ParentObject, button );
					button.AutoToolTip = true;
					button.ToolTipText = ienum.Current.shortcutText;
					// RichToolBarButton button = new RichToolBarButton(ienum.Current.shortcutText, iconIndex);

					switch( ienum.Current.style.Trim().ToLower() )
					{
						case "togglebutton":
							button.CheckOnClick = true;//.Style = ToolBarButtonStyle.ToggleButton;
							break;
						default:
							button.CheckOnClick = false;
							//button.Style = ToolBarButtonStyle.PushButton;
							break;
					}
					toolStrip.Items.Add( button );
					button.Click += new EventHandler( button_Click );
				}
			}
		}

		void button_Click( object sender, EventArgs e )
		{
			if( ((ToolStripButton)sender).Tag != null )
			{
				((ActionInfo)((ToolStripButton)sender).Tag).EventHandler.DynamicInvoke( new object[] { sender, e } );
			}
		}
		#endregion
	}
#endif
}
