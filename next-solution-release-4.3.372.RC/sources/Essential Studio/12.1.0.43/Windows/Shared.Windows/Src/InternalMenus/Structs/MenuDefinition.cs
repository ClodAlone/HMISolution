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
using System.Xml.Serialization;

namespace Syncfusion.Windows.Forms.InternalMenus
{
	/// <exclude/>
	/// <summary>
	/// Defines a Menu by a name and MenuItemStructCollection
	/// </summary>
	[Serializable]
	public class MenuDefinition
	{
		string menuName;
		MenuItemStructCollection collection;

		/// <summary>
		///     Creates an empty MenuDefinition
		/// </summary>
		public MenuDefinition()
		{
		}

		/// <summary>
		///     Constructs a MenuDefinition given the Name and MenuItemStructCollection
		/// </summary>
		/// <param name="mName" type="string">
		///     <para>
		///		Name of the menu         
		///     </para>
		/// </param>
		/// <param name="misCollection" type="Syncfusion.Windows.Forms.InternalMenus.MenuItemStructCollection">
		///     <para>
		///         The MenuItemStructCollection used to create the menu
		///     </para>
		/// </param>
		public MenuDefinition(string mName, MenuItemStructCollection misCollection)
		{
			menuName = mName;
			collection = misCollection;
		}

		#region Properties
		/// <summary>
		///     Name of the contained menu
		/// </summary>
		[XmlAttribute("MenuName")]
		public string MenuName
		{
			get
			{
				if(menuName == null || menuName == string.Empty)
					menuName = "Default Menu";
				return menuName;
			}
			set
			{
				menuName = value;
			}
		}

		/// <summary>
		///     Collection of <see cref="MenuItemStruct"/> objects defining the contained menu items.
		/// </summary>
		public MenuItemStructCollection MenuItems
		{
			get
			{
				return collection;
			}
			set
			{
				collection = value;
			}		
		}
		#endregion
	}
    /// <exclude/>
    /// <summary>
	/// Defines a Toolbar by name and ToolBarItemStructCollection
	/// </summary>
	[Serializable]
	public class ToolBarDefinition
	{
		string toolbarName;
		ToolBarItemStructCollection collection;

		/// <summary>
		///     Creates an empty ToolBarDefinition
		/// </summary>
		public ToolBarDefinition()
		{
		}

		/// <summary>
		///     Creates a ToolBarDefinition given the name and ToolBarItemStructCollection
		/// </summary>
		/// <param name="tName" type="string">
		///     <para>
		///         Name of the menu
		///     </para>
		/// </param>
		/// <param name="tisCollection" type="Syncfusion.Windows.Forms.InternalMenus.ToolBarItemStructCollection">
		///     <para>
		///         ToolBarItemStructCollection used to create the toolbar
		///     </para>
		/// </param>
		/// <returns>
		///     A void value...
		/// </returns>
		public ToolBarDefinition(string tName, ToolBarItemStructCollection tisCollection)
		{
			toolbarName = tName;
			collection = tisCollection;
		}

		#region Properties
		/// <summary>
		///     Name of the defined Toolbar
		/// </summary>
		[XmlAttribute("ToolBarName")]
		public string ToolBarName
		{
			get
			{
				if(toolbarName == null || toolbarName == string.Empty)
					toolbarName = "Default ToolBar";
				return toolbarName;
			}
			set
			{
				toolbarName = value;
			}
		}

		/// <summary>
		///     Collection of <see cref="ToolBarItemStruct"/> objects that defines the items in the toolbar.
		/// </summary>
		public ToolBarItemStructCollection ToolBarItems
		{
			get
			{
				return collection;
			}
			set
			{
				collection = value;
			}		
		}
		#endregion
	}
}
