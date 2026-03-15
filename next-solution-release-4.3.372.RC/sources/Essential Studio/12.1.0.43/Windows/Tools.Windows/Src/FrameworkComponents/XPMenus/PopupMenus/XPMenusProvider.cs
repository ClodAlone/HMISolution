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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// The XPMenusProvider class implements the <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> interface and 
	/// serves as the menu provider for the Syncfusion Essential Tools XPMenus. Controls that have a <see cref="XPMenusProvider"/> 
	/// object set as their menu provider will display a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/> instance.
	/// <seealso cref="Syncfusion.Windows.Forms.IContextMenuProvider"/>
	/// <seealso cref="Sycnfusion.Windows.Forms.Tools.XPMenus.StandardMenusProvider"/>
	/// </summary>	
	public class XPMenusProvider
		: IContextMenuProvider
	{
		#region Fields
		protected PopupMenu popupMenu;
		protected Hashtable textHandlerMap;
        protected bool m_bNeedAddRemoveButton = true;
		#endregion

		#region Properties
		/// <summary>
		/// Gets context menu instance.
		/// </summary>
		private PopupMenu Menu
		{
			get
			{
				if( popupMenu == null )
				{
					InitializeContextMenu();
				}

				return popupMenu;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates an instance of the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.XPMenusProvider"/> class.
		/// </summary>
		public XPMenusProvider()
		{
			this.textHandlerMap = new Hashtable();
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Gets the ContextMenu  items Count.
        /// </summary>
        /// <returns></returns>
        public int GetItemsCount()
        {
            return this.Menu.ParentBarItem.Items.Count;
        }

        /// <summary>
        /// Indicates whether "Add or Remove buttons" is needed.
        /// </summary>
        /// <returns></returns>
        public bool NeedAddRemoveButtons()
        {
            return this.m_bNeedAddRemoveButton;
        }
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.InitializeContextMenu"/>.
		/// </summary>
		public void InitializeContextMenu()
		{
			if( this.popupMenu != null )
				this.DisposeContextMenu();
			this.popupMenu = new PopupMenu();
			this.popupMenu.ParentBarItem = new ParentBarItem();
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetVisualStyle"/>.
		/// </summary>
		public void SetVisualStyle( VisualStyle style )
		{
			this.popupMenu.ParentBarItem.Style = style;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.AddContextMenuItem(string, string, EventHandler)"/>.
		/// </summary>
		public void AddContextMenuItem( String itemtext, EventHandler handler )
		{
			this.AddContextMenuItem( String.Empty, itemtext, handler );
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.AddContextMenuItem(string, string, EventHandler)"/>.
		/// </summary>
		public void AddContextMenuItem( String parentitemtext, String itemtext, EventHandler handler )
		{
            if (itemtext == "&Add or Remove Buttons")
                this.m_bNeedAddRemoveButton = true;

			if( this.popupMenu == null )
			{
				this.popupMenu = new PopupMenu();
				this.popupMenu.ParentBarItem = new ParentBarItem();
			}

			if( parentitemtext == String.Empty )
			{
				if( this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext ) == null )
				{
					BarItem newitem;
					if( handler != null )
					{
						newitem = new BarItem( itemtext, new EventHandler( this.ContextMenuCommandHandler ) );
						this.textHandlerMap.Add( itemtext, handler );
					}
					else
					{
						newitem = new ParentBarItem( itemtext );
					}
					this.popupMenu.ParentBarItem.Items.Add( newitem );
				}
			}
			else
			{
				BarItem parentitem = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, parentitemtext );
				if( ( parentitem != null ) && ( parentitem is ParentBarItem ) )
				{
					ParentBarItem pbi = parentitem as ParentBarItem;
					if( this.RecurseGetMenuItem( pbi, itemtext ) == null )
					{
						BarItem newitem;
						if( handler != null )
						{
							newitem = new BarItem( itemtext, new EventHandler( this.ContextMenuCommandHandler ) );
							this.textHandlerMap.Add( itemtext, handler );
						}
						else
						{
							newitem = new ParentBarItem( itemtext );
						}
						pbi.Items.Add( newitem );
					}
				}
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemImage"/>.
		/// </summary>
		public void SetContextMenuItemImage( String itemtext, ImageList list, int index )
		{
			if( ( list != null ) && ( list.Images != null ) )
			{
				if( index < list.Images.Count )
				{
					if( this.popupMenu != null )
					{
						BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
						if( item != null )
						{
							item.ImageList = list;
							item.ImageIndex = index;
						}
					}
				}
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemShortcut"/>.
		/// </summary>
		public void SetContextMenuItemShortcut( String itemtext, Shortcut key )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
					item.Shortcut = key;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.GetContextMenuItemShortcut"/>.
		/// </summary>
		public Shortcut GetContextMenuItemShortcut( String itemtext )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
					return item.Shortcut;
			}
			return Shortcut.None;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemChecked"/>.
		/// </summary>
		public void SetContextMenuItemChecked( String itemtext, bool bchecked )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
					item.Checked = bchecked;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.GetContextMenuItemChecked"/>.
		/// </summary>
		public bool GetContextMenuItemChecked( String itemtext )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
					return item.Checked;
			}
			return false;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemEnabled"/>.
		/// </summary>
		public void SetContextMenuItemEnabled( String itemtext, bool benabled )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
					item.Enabled = benabled;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.GetContextMenuItemEnabled"/>.
		/// </summary>
		public bool GetContextMenuItemEnabled( String itemtext )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
					return item.Enabled;
			}
			return false;
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemSeparator"/>.
		/// </summary>
		public void SetContextMenuItemSeparator( String itemtext, bool binsertseparator )
		{
			if( this.popupMenu != null )
			{
				BarItem item = this.RecurseGetMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( item != null )
				{
					ParentBarItem parentitem = this.RecurseGetParentMenuItem( this.popupMenu.ParentBarItem, itemtext );
					if( binsertseparator == true )
						parentitem.BeginGroupAt( item );
					else
						parentitem.RemoveGroupAt( item );
				}
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.RemoveContextMenuItem"/>.
		/// </summary>
		public void RemoveContextMenuItem( String itemtext )
		{
            if (itemtext == "&Add or Remove Buttons")
            {
                this.m_bNeedAddRemoveButton = false;
                return;
            }
			if( this.popupMenu != null )
			{
				ParentBarItem parentitem = this.RecurseGetParentMenuItem( this.popupMenu.ParentBarItem, itemtext );
				if( parentitem != null )
				{
					BarItem item = this.RecurseGetMenuItem( parentitem, itemtext );
					parentitem.Items.Remove( item );
				}
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.ShowContextMenu"/>.
		/// </summary>
		public void ShowContextMenu( Control owner, Point pt )
		{
			if( this.popupMenu != null )
				this.popupMenu.Show( owner, pt );
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.DisposeContextMenu"/>.
		/// </summary>
		public void DisposeContextMenu()
		{
			if( this.popupMenu != null )
			{
				this.RecurseDisposeMenuItems( this.popupMenu.ParentBarItem );
				this.popupMenu.Dispose();
			}
			this.textHandlerMap.Clear();
		}
		/// <summary>
		/// Clears all menu entries.
		/// </summary>
		public void Clear()
		{
			if( this.popupMenu != null )
			{
				this.popupMenu.ParentBarItem.Items.Clear();
			}
			this.textHandlerMap.Clear();
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when menu is popped up.
		/// </summary>
		public event EventHandler Popup
		{
			add
			{
				this.Menu.Popup += value;
			}
			remove
			{
				this.Menu.Popup -= value;
			}
		}
		/// <summary>
		/// Occurs when menu is collapsed.
		/// </summary>
		public event EventHandler Collapse
		{
			add
			{
				this.Menu.Collapse += value;
			}
			remove
			{
				this.Menu.Collapse -= value;
			}
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		protected void ContextMenuCommandHandler( object sender, EventArgs args )
		{
			String itemtext = ( sender as BarItem ).Text;
			EventHandler cmdhandler = new EventHandler( this.textHandlerMap[ itemtext ] as EventHandler );
			cmdhandler( new ContextMenuItem( this, itemtext ), args );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentitem"></param>
		/// <param name="itemtext"></param>
		/// <returns></returns>
		protected bool RecurseContainsMenuItem( ParentBarItem parentitem, String itemtext )
		{
			foreach( BarItem item in parentitem.Items )
			{
				if( item.Text == itemtext )
					return true;
				if( item is ParentBarItem )
				{
					if( this.RecurseContainsMenuItem( item as ParentBarItem, itemtext ) == true )
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentitem"></param>
		/// <param name="itemtext"></param>
		/// <returns></returns>
		protected BarItem RecurseGetMenuItem( ParentBarItem parentitem, String itemtext )
		{
			if( ( parentitem != null ) && ( parentitem.Items != null ) )
			{
				foreach( BarItem item in parentitem.Items )
				{
					if( item.Text == itemtext )
						return item;
					if( item is ParentBarItem )
					{
						BarItem reqditem = this.RecurseGetMenuItem( item as ParentBarItem, itemtext );
						if( reqditem != null )
							return reqditem;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentitem"></param>
		/// <param name="itemtext"></param>
		/// <returns></returns>
		protected ParentBarItem RecurseGetParentMenuItem( ParentBarItem parentitem, String itemtext )
		{
			foreach( BarItem item in parentitem.Items )
			{
				if( item.Text == itemtext )
					return parentitem;
				if( item is ParentBarItem )
				{
					ParentBarItem reqdparent = this.RecurseGetParentMenuItem( item as ParentBarItem, itemtext );
					if( reqdparent != null )
						return reqdparent;
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentitem"></param>
		protected void RecurseDisposeMenuItems( ParentBarItem parentitem )
		{
			BarItem[] items = new BarItem[ parentitem.Items.Count ];
			int i = 0;
			foreach( BarItem item in parentitem.Items )
				items[ i++ ] = item;
			parentitem.Items.Clear();
			for( i = 0; i < items.GetLength( 0 ); i++ )
			{
				BarItem itemtodispose = items[ i ];
				if( itemtodispose is ParentBarItem )
					this.RecurseDisposeMenuItems( itemtodispose as ParentBarItem );
				itemtodispose.Dispose();
			}
		}
		#endregion
	}
}
