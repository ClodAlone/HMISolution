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
using System.Drawing;


namespace Syncfusion.Windows.Forms
{
	/// <summary>
	/// Defines an interface for classes that support a BeginUpdate / EndUpdate pattern.
	/// </summary>
	public interface ISupportUpdating
	{
		/// <summary>
		/// Suspends updating the component. An internal counter will be increased if called multiple times.
		/// </summary>
		void BeginUpdate();
		/// <summary>
		/// Resumes updating the component. If <see cref="BeginUpdate"/> was called multiple times, an internal counter is decreased.
		/// </summary>
		void EndUpdate();
		/// <summary>
		/// Indicates whether <see cref="BeginUpdate"/> was called.
		/// </summary>
		bool Updating { get; }
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IStatusBarAdv
	{
	}
	/// <summary>
	/// The IContextMenuProvider interface provides Essential Studio controls with a high-level API for creating and 
	/// working with context menus. Subscribing to this interface allows the Essential Studio controls to 
	/// seamlessly switch between the standard .NET <see cref="System.Windows.Forms.ContextMenu"/> and the 
    /// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/> classes depending on whether the 
	/// Essential Tools library is available or not.
	/// <seealso cref="Syncfusion.Windows.Forms.StandardMenusProvider"/>
	/// <seealso cref="Syncfusion.Windows.Forms.Tools.XPMenus.XPMenusProvider"/>
	/// <seealso cref="Syncfusion.Windows.Forms.ContextMenuItem"/>
	/// </summary>
	public interface IContextMenuProvider
	{
		/// <summary>
		/// Creates a new instance of the context menu object managed by this provider.	
		/// </summary>
		/// <remarks>
		/// If the provider contains a previously initialized context menu, then the existing menu will be disposed 
		/// before creating the new menu. 
		/// </remarks>
		void InitializeContextMenu();

        /// <summary>
        /// Gets the Menu items count.
        /// </summary>
        /// <returns></returns>
        int GetItemsCount();

        /// <summary>
        /// Indicates whether "Add or Remove buttons" is needed.
        /// </summary>
        /// <returns></returns>
        bool NeedAddRemoveButtons();

		/// <summary>
		/// Sets the visual style for the context menu.
		/// </summary>
		/// <param name="style">A <see cref="Syncfusion.Windows.Forms.VisualStyle"/> value.</param>
		void SetVisualStyle(VisualStyle style);

		/// <summary>
		/// Creates a new top-level menu item.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="handler">The <see cref="System.EventHandler"/> that will handle the menu item Click event.</param>
		void AddContextMenuItem(String itemtext, EventHandler handler);

		/// <summary>
		/// Creates a new menu item and adds it to the specified parent menu item.
		/// </summary>
		/// <param name="parentitem">A <see cref="System.String"/> value representing the parent menu item.</param>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="handler">The <see cref="System.EventHandler"/> that will handle the menu item Click event.</param>
		void AddContextMenuItem(String parentitem, String itemtext, EventHandler handler);

		/// <summary>
		/// Sets the menu item image.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="imagelist">The <see cref="System.Windows.Forms.ImageList"/> containing the image.</param>
		/// <param name="image">The zero-based image index.</param>
		void SetContextMenuItemImage(String itemtext, ImageList imagelist, int image);

		/// <summary>
		/// Sets a shortcut key for the menu item.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="key">The <see cref="System.Windows.Forms.Shortcut"/> key for the menu item.</param>
		void SetContextMenuItemShortcut(String itemtext, Shortcut key);

		/// <summary>
		/// Returns the menu item's shortcut key.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <returns>A <see cref="System.Windows.Forms.Shortcut"/> key value.</returns>
		Shortcut GetContextMenuItemShortcut(String itemtext);

		/// <summary>
		/// Sets the menu item's Checked property to the specified value.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="bchecked">The boolean value to be set.</param>
		void SetContextMenuItemChecked(String itemtext, bool bchecked);

		/// <summary>
		/// Gets the menu item's Checked property.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <returns>A boolean value.</returns>
		bool GetContextMenuItemChecked(String itemtext);

		/// <summary>
		/// Sets the menu item's Enabled property to the specified value.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="benabled">The boolean value to be set.</param>
		void SetContextMenuItemEnabled(String itemtext, bool benabled);

		/// <summary>
		/// Indicates the state of the menu item's Enabled property.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <returns>A boolean value.</returns>
		bool GetContextMenuItemEnabled(String itemtext);

		/// <summary>
		/// Inserts or removes a separator before the specified menu item's position.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		/// <param name="binsertseparator">True to insert a new separator; False to remove an existing separator.</param>
		void SetContextMenuItemSeparator(String itemtext, bool binsertseparator);

		/// <summary>
		/// Removes the specified context menu item.
		/// </summary>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the menu item.</param>
		void RemoveContextMenuItem(String itemtext);

		/// <summary>
		/// Displays the context menu at the specified position.
		/// </summary>
		/// <param name="owner">A <see cref="System.Windows.Forms.Control"/> object that specifies the control with which this context menu is associated.</param>
		/// <param name="pt">A <see cref="System.Drawing.Point"/> object that specifies the coordinates at which to display the menu.</param>
		void ShowContextMenu(Control owner, Point pt);

		/// <summary>
		/// Disposes the context menu associated with this provider.
		/// </summary>
		void DisposeContextMenu();

		/// <summary>
		/// Clears all menu items.
		/// </summary>
		void Clear();

		/// <summary>
		/// Occurs when menu is popped up.
		/// </summary>
		event EventHandler Popup;

		/// <summary>
		/// Occurs when menu is collapsed.
		/// </summary>
		event EventHandler Collapse;
	}
	/// <summary>
	/// The ContextMenuItem class is used by the Essential Studio menu providers - classes implementing the 
	/// <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> interface - for providing information to the context menu 
	/// command handler about the menu item that triggered the Click event.
	/// <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/>
	/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.XPMenusProvider"/>
	/// </summary>
	public class ContextMenuItem
	{
		protected IContextMenuProvider menuProvider;
		protected String menuItemText;

		/// <summary>
		/// Returns the menu provider instance.
		/// </summary>
		/// <value>A <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> instance.</value>
		public IContextMenuProvider ContextMenuProvider
		{
			get { return this.menuProvider; }
		}

		/// <summary>
		/// Returns the text representing the context menu item.
		/// </summary>
		/// <value>A <see cref="System.String"/> value.</value>
		public string ContextMenuItemText
		{
			get { return this.menuItemText; }
		}

		/// <summary>
		/// Creates an instance of the <see cref="Syncfusion.Windows.Forms.ContextMenuItem"/> class.
		/// </summary>
		/// <param name="provider">A <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> instance representing the menu provider.</param>
		/// <param name="itemtext">A <see cref="System.String"/> value representing the context menu item.</param>
		public ContextMenuItem(IContextMenuProvider provider, String itemtext)
		{
			this.menuProvider = provider;
			this.menuItemText = itemtext;
		}
	}


}
