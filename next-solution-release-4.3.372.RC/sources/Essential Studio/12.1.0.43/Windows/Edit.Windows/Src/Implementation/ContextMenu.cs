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
using System.Windows.Forms;
using System.Collections;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Implementation
{
	/// <summary>
	/// Summary description for ContextMenu.
	/// </summary>
	public class ContextMenuManager
		: IDisposable
	{
		#region Class constants
		/// <summary>
		/// Name of the menu item that will be displayed as divider.
		/// </summary>
		private const string DEF_STR_MNU_DIVIDER = "-";
		#endregion

		#region Class members
		/// <summary>
		/// Parent control.
		/// </summary>
		private Control m_parent;
		/// <summary>
		/// Context menu.
		/// </summary>
		IContextMenuProvider menuProvider;
		/// <summary>
		/// Specifies, whether context menu is enabled.
		/// </summary>
		private bool m_bEnabled = true;
		/// <summary>
		/// Insert separator next time AddMenuItem is called.
		/// </summary>
		private bool m_separator = false;
		/// <summary>
		/// Indicates whether menu is shown.
		/// </summary>
		private bool m_bIsShown;
		/// <summary>
		/// Specifies whether the ContextMenuProvider is of XPMenusProvider type.
		/// </summary>
		private bool isXPMenusProvider = false;
		#endregion

		#region Class Events
		/// <summary>
		/// Event, that is raised, when user should fill menu with menu items.
		/// </summary>
		public event EventHandler FillMenu;
		#endregion

		#region Class Properties
		/// <summary>
		/// GET, SET flag that specifies, whether context menu is enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return m_bEnabled;
			}
			set
			{
				m_bEnabled = value;
			}
		}
		/// <summary>
		/// The IContextMenuProvider interface provides Essential Suite controls with a high-level API for creating and 
		/// working with context menus. Subscribing to this interface allows the Essential Suite controls to 
		/// seamlessly switch between the standard .NET <see cref="System.Windows.Forms.ContextMenu"/> and the 
		/// Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu classes depending on the whether the 
		/// Essential Tools library is available or not.
		/// <seealso cref="Syncfusion.Windows.Forms.StandardMenusProvider"/>
		/// <seealso cref="Syncfusion.Windows.Forms.ContextMenuItem"/>
		/// </summary>
		public IContextMenuProvider ContextMenuProvider
		{
			get
			{
				if( menuProvider == null )
				{
					this.menuProvider = Syncfusion.Windows.Forms.MenuProviderFactory.CreateContextMenuProvider();
					this.menuProvider.InitializeContextMenu();
					this.menuProvider.SetVisualStyle( VisualStyle.Office2003 );

					menuProvider.Popup += new EventHandler( OnMenuProviderPopup );
					menuProvider.Collapse += new EventHandler( OnMenuProviderCollapse );
				}

				return menuProvider;
			}
			set
			{
				menuProvider = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether menu is shown.
		/// </summary>
		public bool IsShown
		{
			get
			{
				return m_bIsShown;
			}
		}
		/// <summary>
		/// Specifies whether the ContextMenuProvider is of XPMenusProvider type.
		/// </summary>
		public bool IsXPMenusProvider
		{
			get 
			{
				if (!isXPMenusProvider)
				{
					System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					foreach (System.Reflection.Assembly asm in assemblies)
					{
						string name = asm.GetName().Name;
						if (name == "Syncfusion.Tools.Windows")
						{
							Type type = asm.GetType("Syncfusion.Windows.Forms.Tools.XPMenus.XPMenusProvider");
							if (this.ContextMenuProvider.GetType() == type)
							{
								isXPMenusProvider = true;
								break;
							}
						}
					}
				}
				return isXPMenusProvider; 
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates and itializes new context menu processor.
		/// </summary>
		/// <param name="parent">Control, context menu should be attached to.</param>
		public ContextMenuManager( Control parent )
		{
			if( parent == null )
				throw new ArgumentNullException( "parent" );

			m_parent = parent;
			m_parent.MouseUp += new MouseEventHandler( ParentMouseUp );
		}
		/// <summary>
		/// Disposes all used resources.
		/// </summary>
		public void Dispose()
		{
			if( m_parent != null )
			{
				m_parent = null;
			}
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Clears menu.
		/// </summary>
		public void ClearMenu()
		{
			ContextMenuProvider.DisposeContextMenu();
			menuProvider = null;
		}
		/// <summary>
		/// Appends new menu item to menu.
		/// </summary>
		/// <param name="name">Name of the menu item.</param>
		/// <param name="handlerClick">Handler of the click event of the item. Null if item will have children.</param>
		public void AddMenuItem( string name, EventHandler handlerClick )
		{
			if( name == string.Empty || name == null )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_119, "name" );

			ContextMenuProvider.AddContextMenuItem( name, handlerClick );
			if( m_separator )
			{
				ContextMenuProvider.SetContextMenuItemSeparator( name, true );
				m_separator = false;
			}
		}

		/// <summary>
		/// Appends new menu item to menu.
		/// </summary>
		/// <param name="name">Name of the menu item.</param>
		/// <param name="handlerClick">Handler of the click event of the item.</param>
		/// <param name="enabled">The boolean value for the menu item's Enabled property to be set.</param>
		public void AddMenuItem( string name, EventHandler handlerClick, bool enabled )
		{
			AddMenuItem( name, handlerClick );
			ContextMenuProvider.SetContextMenuItemEnabled( name, enabled );
		}
		/// <summary>
		/// Appends new divider to menu.
		/// </summary>
		public void AddSeparator()
		{
			m_separator = true;  // Insert separator next time AddMenuItem is called.
		}
		#endregion

		#region Class Helper Methods
		/// <summary>
		/// Initializes context menu and shows it.
		/// </summary>
		protected virtual void ShowContextMenu()
		{
			if( !this.Enabled )
				return;

			ClearMenu();

			if( FillMenu != null )
				FillMenu( this, EventArgs.Empty );

			IContextMenuProvider menu = this.ContextMenuProvider;

			if (m_parent.RightToLeft == RightToLeft.Yes)
				ApplyRTLToContextMenuProvider();

			menu.ShowContextMenu( m_parent, m_parent.PointToClient( Control.MousePosition ) );
		}
		/// <summary>
		/// Applies RightToLeft property to the ContextMenuProvider.
		/// </summary>
		private void ApplyRTLToContextMenuProvider()
		{
			System.Reflection.BindingFlags flag = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
			System.Reflection.PropertyInfo info = null;

			if (this.ContextMenuProvider is StandardMenusProvider)
			{
				info = typeof(StandardMenusProvider).GetProperty("Menu",flag);
				ContextMenu menu = info.GetValue(this.ContextMenuProvider,null) as ContextMenu;

				if (menu != null)
					menu.RightToLeft = RightToLeft.Yes;
			}
			else if (IsXPMenusProvider)
			{
				// XPMenusProvider.PopupMenu.MenuGrid.RightToLeft = RightToLeft.Yes;
				// types are not available to access the XPMenusProvider.PopupMenu.MenuGrid.RightToLeft
			}
		}
		/// <summary>
		/// Handles pressing of the right button.
		/// </summary>
		/// <param name="sender">Sender of the event.</param>
		/// <param name="e">Mouse event arguments.</param>
		private void ParentMouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right && m_bEnabled )
			{
				ShowContextMenu();
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMenuProviderPopup( object sender, EventArgs e )
		{
			m_bIsShown = true;			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMenuProviderCollapse( object sender, EventArgs e )
		{
			m_bIsShown = false;
		}
		#endregion
	}
}