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
using System.Runtime.Remoting;
using System.Reflection;

namespace Syncfusion.Windows.Forms
{
	#region *** MenuProviderFactory
	/// <summary>
	/// Factory class for creating the context menu provider. If the Syncfusion Essential Tools library is present, the MenuProviderFactory will 
	/// create an instance of the <see cref="Syncfusion.Windows.Forms.InternalMenus.MenuFactory"/> type and if not it returns an instance of 
	/// the <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/> class.
	/// <seealso cref="Syncfusion.Windows.Forms.IContextMenuProvider"/>
	/// </summary>
	public class MenuProviderFactory
	{
		#region Public Methods
		/// <summary>
		/// Creates the standard or XPMenus context menu provider.
		/// </summary>
		/// <returns>A <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> instance.</returns>
		public static IContextMenuProvider CreateContextMenuProvider()
		{
			IContextMenuProvider iprovider = null;

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach( Assembly asm in assemblies )
			{
				string name = asm.GetName().Name;
				if( name == "Syncfusion.Tools.Windows" )
				{
					iprovider = ( Syncfusion.Windows.Forms.IContextMenuProvider )Activator.CreateInstance(
						asm.GetType( "Syncfusion.Windows.Forms.Tools.XPMenus.XPMenusProvider" ) );
				}
			}

			if( iprovider == null )
			{
				// Essential Tools is not available. Use the Syncfusion.Windows.Forms.StandardMenusProvider type.
				iprovider = new Syncfusion.Windows.Forms.StandardMenusProvider();
			}

			return iprovider;
		}
		#endregion
	}
	#endregion

	#region *** StandardMenusProvider
	/// <summary>
	/// The StandardMenusProvider class implements the <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> interface and 
	/// serves as the menu provider for the standard .NET context menus. Controls that have a <see cref="StandardMenusProvider"/> 
	/// object set as their menu provider will display a <see cref="System.Windows.Forms.ContextMenu"/> instance.
	/// <seealso cref="Syncfusion.Windows.Forms.IContextMenuProvider"/>
	/// <seealso cref="Sycnfusion.Windows.Forms.XPMenus.XPMenusProvider"/>
	/// </summary>	
	public class StandardMenusProvider
		: IContextMenuProvider
	{
		#region Fields
		protected ContextMenu cntxtMenu;
		protected Hashtable handlerMap;
        protected bool m_bNeedAddRemoveButton = true;
		#endregion

		#region Properties
		/// <summary>
		/// Gets context menu instance.
		/// </summary>
		internal ContextMenu Menu
		{
			get
			{
				if( cntxtMenu == null )
				{
					InitializeContextMenu();
				}

				return cntxtMenu;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates an instance of the <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/> class.
		/// </summary>
		public StandardMenusProvider()
		{
			this.handlerMap = new Hashtable();
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Gets the ContextMenu  items Count.
        /// </summary>
        /// <returns></returns>
        public int GetItemsCount()
        {
            return this.Menu.MenuItems.Count;
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
			if( this.cntxtMenu != null )
				this.DisposeContextMenu();
			this.cntxtMenu = new ContextMenu();
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetVisualStyle"/>.
		/// </summary>
		public void SetVisualStyle( VisualStyle style )
		{
			// No imp
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.AddContextMenuItem(string, System.EventHandler)"/>.
		/// </summary>
		public void AddContextMenuItem( String itemtext, EventHandler handler )
		{
			this.AddContextMenuItem( String.Empty, itemtext, handler );
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.AddContextMenuItem(string, string, System.EventHandler)"/>.
		/// </summary>
		public void AddContextMenuItem( String parentitemtext, String itemtext, EventHandler handler )
		{
            if (itemtext == "&Add or Remove Buttons")
                this.m_bNeedAddRemoveButton = true;

			if( this.cntxtMenu == null )
				this.cntxtMenu = new ContextMenu();
			if( parentitemtext == String.Empty )
			{
				if( this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext ) == null )
				{
					this.handlerMap.Add( itemtext, handler );
					this.cntxtMenu.MenuItems.Add( new MenuItem( itemtext, new EventHandler( this.ContextMenuCommandHandler ) ) );
				}
			}
			else
			{
				MenuItem parent = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, parentitemtext );
				if( ( parent != null ) && ( this.RecurseGetMenuItem( parent.MenuItems, itemtext ) == null ) )
				{
					this.handlerMap.Add( itemtext, handler );
					parent.MenuItems.Add( new MenuItem( itemtext, new EventHandler( this.ContextMenuCommandHandler ) ) );
				}
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemImage"/>.
		/// </summary>
		public void SetContextMenuItemImage( String itemtext, ImageList list, int index )
		{
			// No imp
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.SetContextMenuItemShortcut"/>.
		/// </summary>
		public void SetContextMenuItemShortcut( String itemtext, Shortcut key )
		{
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
				if( item != null )
					item.Shortcut = key;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.GetContextMenuItemShortcut"/>.
		/// </summary>
		public Shortcut GetContextMenuItemShortcut( String itemtext )
		{
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
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
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
				if( item != null )
					item.Checked = bchecked;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.GetContextMenuItemChecked"/>.
		/// </summary>
		public bool GetContextMenuItemChecked( String itemtext )
		{
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
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
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
				if( item != null )
					item.Enabled = benabled;
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.GetContextMenuItemEnabled"/>.
		/// </summary>
		public bool GetContextMenuItemEnabled( String itemtext )
		{
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
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
			if( this.cntxtMenu != null )
			{
				MenuItem item = this.RecurseGetMenuItem( this.cntxtMenu.MenuItems, itemtext );
				if( item != null )
				{
					// Insert the separator before this menu item.
					Menu.MenuItemCollection parentcollection = this.RecurseGetParentCollection( this.cntxtMenu.MenuItems, itemtext );
					if( binsertseparator == true )
					{
						parentcollection.Remove( item );
						parentcollection.Add( "-" );
						parentcollection.Add( item );
					}
					else
					{
						int itemindex = parentcollection.IndexOf( item );
						if( itemindex > 0 )
						{
							MenuItem previtem = parentcollection[ itemindex - 1 ];
							if( previtem.Text == "-" )
								parentcollection.Remove( previtem );
						}
					}
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
			if( this.cntxtMenu != null )
			{
				Menu.MenuItemCollection parentcollection = this.RecurseGetParentCollection( this.cntxtMenu.MenuItems, itemtext );
				if( parentcollection != null )
				{
					MenuItem item = this.RecurseGetMenuItem( parentcollection, itemtext );
					parentcollection.Remove( item );
				}
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.ShowContextMenu"/>.
		/// </summary>
		public void ShowContextMenu( Control owner, Point pt )
		{
			if( this.cntxtMenu != null )
				this.cntxtMenu.Show( owner, pt );
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.IContextMenuProvider.DisposeContextMenu"/>.
		/// </summary>
		public void DisposeContextMenu()
		{
			if( this.cntxtMenu != null )
			{
				this.RecurseDisposeMenuItems( this.cntxtMenu.MenuItems );
				this.cntxtMenu.Dispose();
			}
			this.handlerMap.Clear();
		}
		/// <summary>
		/// Clears all menu items.
		/// </summary>
		public void Clear()
		{
			if( this.cntxtMenu != null )
			{
				this.cntxtMenu.MenuItems.Clear();
				this.handlerMap.Clear();
			}
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
			String itemtext = ( sender as MenuItem ).Text;
			EventHandler cmdhandler = new EventHandler( this.handlerMap[ itemtext ] as EventHandler );
			cmdhandler( new ContextMenuItem( this, itemtext ), args );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="itemtext"></param>
		/// <returns></returns>
		protected MenuItem RecurseGetMenuItem( Menu.MenuItemCollection collection, String itemtext )
		{
			if( collection != null )
			{
				foreach( MenuItem item in collection )
				{
					if( item.Text == itemtext )
						return item;
					if( item.MenuItems != null )
					{
						MenuItem childitem = this.RecurseGetMenuItem( item.MenuItems, itemtext );
						if( childitem != null )
							return childitem;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="itemtext"></param>
		/// <returns></returns>
		protected Menu.MenuItemCollection RecurseGetParentCollection( Menu.MenuItemCollection collection, String itemtext )
		{
			foreach( MenuItem item in collection )
			{
				if( item.Text == itemtext )
					return collection;
				Menu.MenuItemCollection childitemcollection = this.RecurseGetParentCollection( item.MenuItems, itemtext );
				if( childitemcollection != null )
					return childitemcollection;
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		protected void RecurseDisposeMenuItems( Menu.MenuItemCollection collection )
		{
			MenuItem[] items = new MenuItem[ collection.Count ];
			int i = 0;
			foreach( MenuItem item in collection )
				items[ i++ ] = item;
			collection.Clear();
			for( i = 0; i < items.GetLength( 0 ); i++ )
			{
				MenuItem itemtodispose = items[ i ];
				this.RecurseDisposeMenuItems( itemtodispose.MenuItems );
				itemtodispose.Dispose();
			}
		}
		#endregion
	}
	#endregion
}